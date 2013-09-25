using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuDocs.Tools
{
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
}