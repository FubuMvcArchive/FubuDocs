using FubuCore.Util;
using FubuDocs.Topics;

namespace FubuDocs.Tools
{
    public class TopicTokenCache : ITopicTokenCache
    {
        private readonly Cache<string, TopicToken> _tokens;

        public TopicTokenCache()
        {
            _tokens = new Cache<string, TopicToken>(name => {
                var project = TopicGraph.AllTopics.ProjectFor(name);

                return new TopicToken(project.Index)
                {
                    Key = "index" // important for making the UI work
                };
            });
        }

        public TopicToken TopicStructureFor(string projectName)
        {
            return _tokens[projectName];
        }

        public void RewriteTopicStructure(string projectName, TopicToken newStructure)
        {
            var original = _tokens[projectName];

            var collector = new DeltaCollector(original, newStructure);
            collector.ExecuteDeltas();

            // Want the new one in on the next try.
            _tokens.Remove(projectName);
        }
    }
}