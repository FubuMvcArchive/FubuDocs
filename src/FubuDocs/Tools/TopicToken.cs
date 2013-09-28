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
            Key = topic.Key.Split('/').Last().Split('.').Last();
            FullKey = topic.Key;
            Title = topic.Title;
            Url = topic.Url.Split('/').Last();
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

        public IEnumerable<IDelta> DetermineDeltas(TopicToken originalRoot)
        {
            var original = originalRoot.Find(Id);
            if (original == null)
            {
                yield return new NewTopic(this);
            }
            else
            {
                if (original.File != File)
                {
                    yield return new MoveTopic(original.File, File);
                }

                if (original.Title != Title)
                {
                    yield return new RewriteTitle(File, Title);
                }

                if (original.Url != Url)
                {
                    yield return new RewriteUrl(File, Url);
                }
            }
        } 

        public TopicToken()
        {
        }

        public TopicToken Clone()
        {
            var clone = new TopicToken
            {
                Url = Url,
                Key = Key,
                FullKey = FullKey,
                Title = Title,
                File = File,
                Folder = Folder,
                Id = Id
            };

            clone.Children.AddRange(Children.Select(x => x.Clone()));
            clone.assignOrders();

            return clone;
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
            var token = new TopicToken(root.Index);

            token.Key = "index";

            return token;
        }

        public void DeterminePaths(string containingFolder)
        {
            assignOrders();
            var filename = "{0}.{1}".ToFormat(Order, Key);

            if (Children.Any())
            {
                var folder = containingFolder.AppendPath(filename);
                File = folder.AppendPath("index.spark");

                Children.Each(x => x.DeterminePaths(folder));
            }
            else
            {
                File = containingFolder.AppendPath(filename + ".spark");
            }
        }
    }
}