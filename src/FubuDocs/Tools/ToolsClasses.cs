using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuDocs.Topics;

namespace FubuDocs.Tools
{
    public class TopicToken
    {
        public static readonly IFileSystem FileSystem = new FileSystem();

        public readonly IList<TopicToken> Children = new List<TopicToken>();

        public TopicToken(Topic topic)
        {
            File = topic.File.FilePath.ToFullPath();
            Key = Path.GetFileNameWithoutExtension(File);
            FullKey = topic.Key;
            Title = topic.Title;
            Url = topic.Url;
            Folder = File.ParentDirectory();

            var child = topic.FirstChild;
            while (child != null)
            {
                Children.Add(new TopicToken(child));
                child = child.NextSibling;
            }

            assignOrders();
        }

        private void assignOrders()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Order = i + 1;
            }

            Children.Each(x => x.assignOrders());
        }

        public IEnumerable<IDelta> DetermineDeltas(TopicToken original)
        {
            /* TODO --
             * 1.) if original is null, return NewTopic
             * 2.) if original is not null, and the path is different, return MoveContent
             * 3.) if original is not null and the title is different
             * 4.) if original is not null and the url is different
             */

              throw new NotImplementedException();
        } 

        public TopicToken()
        {
        }

        public void AddChild(TopicToken child)
        {
            Children.Add(child);
            assignOrders();
        }

        public TopicToken Find(Guid id)
        {
            return All().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<TopicToken> All()
        {
            yield return this;

            foreach (var descendent in Descendents())
            {
                yield return descendent;
            }
        }

        public IEnumerable<TopicToken> Descendents()
        {
            foreach (var child in Children)
            {
                yield return child;

                foreach (var descendent in child.Descendents())
                {
                    yield return descendent;
                }
            }
        }

        public string File;

        public TopicToken FindChild(string key)
        {
            return Children.FirstOrDefault(x => x.Key == key);
        }

        public Guid Id = Guid.NewGuid();
        public string Key;
        public string Title;
        public string Url;
        public int Order;
        public string FullKey;
        public string Folder;

        public bool IsIndex
        {
            get
            {
                return Path.GetFileNameWithoutExtension(File).EqualsIgnoreCase("index");
            }
        }

        protected bool Equals(TopicToken other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TopicToken) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static TopicToken LoadIndex(string directory)
        {
            var root = TopicLoader.LoadFromFolder(directory);
            return new TopicToken(root.Index);
        }

        public void DeterminePaths(string folder)
        {
            throw new NotImplementedException();
        }
    }

    public class DeltaCollector
    {
        public readonly IList<IDelta> _deltas = new List<IDelta>(); 

        public DeltaCollector(TopicToken original, TopicToken @new)
        {
            @new.DeterminePaths(original.Folder);

            findDeletedTopics(original, @new);
            findNewAndDeletedFolders(original, @new);

            @new.All().Each(x => {
                var existing = original.Find(x.Id); // will be null if new
                var topicDeltas = x.DetermineDeltas(existing);

                _deltas.AddRange(topicDeltas);
            });

        }

        private void findNewAndDeletedFolders(TopicToken original, TopicToken @new)
        {
            var oldFolders = original.All().Select(x => x.Folder).Distinct().ToList();
            var newFolders = @new.All().Select(x => x.Folder).Distinct().ToList();

            var deletedFolders = oldFolders.Where(x => !newFolders.Contains(x)).Select(x => new DeleteFolder(x));
            _deltas.AddRange(deletedFolders);

            var addedFolders = newFolders.Where(x => !oldFolders.Contains(x)).Select(x => new CreateFolder(x));
            _deltas.AddRange(addedFolders);
        }

        private void findDeletedTopics(TopicToken original, TopicToken @new)
        {
            var deletedTopics = original.All().Where(x => @new.Find(x.Id) == null)
                .Select(x => new DeleteTopic(x));
            _deltas.AddRange(deletedTopics);
        }

        public IEnumerable<IDelta> OrderedDeltas()
        {
            return groups().SelectMany(x => x).ToArray();
        }

        private IEnumerable<IEnumerable<IDelta>> groups()
        {
            yield return _deltas.OfType<DeleteTopic>();
            yield return _deltas.OfType<DeleteFolder>();
            yield return _deltas.OfType<CreateFolder>().OrderBy(x => x.Folder);
            yield return _deltas.OfType<NewTopic>();
            yield return _deltas.OfType<MoveTopic>();
            yield return _deltas.OfType<RewriteTitle>();
            yield return _deltas.OfType<RewriteUrl>();
        }

        public void ExecuteDeltas()
        {
            var deltas = OrderedDeltas();

            deltas.Each(x => x.Prepare());
            deltas.Each(x => {
                Console.WriteLine(x);
                x.Execute();
            });
        }
    }

    public interface IDelta
    {
        void Prepare();
        void Execute();
    }

    public class RewriteTitle : IDelta
    {
        private readonly string _file;
        private readonly string _title;

        public RewriteTitle(string file, string title)
        {
            _file = file;
            _title = title;
        }

        public void Prepare()
        {
            
        }

        public void Execute()
        {
            var contents = TopicToken.FileSystem.ReadStringFromFile(_file);
            var comments = TopicBuilder.FindComments(contents);

            var existingTitle = comments.FirstOrDefault(x => x.StartsWith("Title"));
            if (existingTitle == null)
            {
                contents = "<!-- Topic: {0} -->".ToFormat(_title) + Environment.NewLine + contents;
            }
            else
            {
                contents = contents.Replace(existingTitle, "Title: " + _title);
            }

            TopicToken.FileSystem.WriteStringToFile(_file, contents);
        }
    }

    public class RewriteUrl : IDelta
    {
        private readonly string _file;
        private readonly string _url;

        public RewriteUrl(string file, string url)
        {
            _file = file;
            _url = url;
        }

        public void Prepare()
        {

        }

        public void Execute()
        {
            var contents = TopicToken.FileSystem.ReadStringFromFile(_file);
            var comments = TopicBuilder.FindComments(contents);

            var existingUrl = comments.FirstOrDefault(x => x.StartsWith("Url"));
            if (existingUrl == null)
            {
                contents = "<!-- Topic: {0} -->".ToFormat(_url) + Environment.NewLine + contents;
            }
            else
            {
                contents = contents.Replace(existingUrl, "Url: " + _url);
            }

            TopicToken.FileSystem.WriteStringToFile(_file, contents);
        }
    }
}