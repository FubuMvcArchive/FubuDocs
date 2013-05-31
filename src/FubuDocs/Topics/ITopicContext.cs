using System;
using FubuMVC.Core.Http;
using System.Linq;

namespace FubuDocs.Topics
{
    public interface ITopicContext
    {
        Topic Current { get; }
        ProjectRoot Project { get; }

        string File { get; }
    }

    public class TopicContext : ITopicContext
    {
        private readonly Lazy<Topic> _topic;

        public TopicContext(ICurrentChain currentChain)
        {
            _topic = new Lazy<Topic>(() => {
                var node = currentChain.OriginatingChain.OfType<TopicBehaviorNode>().FirstOrDefault();
                return node == null ? null : node.Topic;
            });
        }

        public Topic Current
        {
            get { return _topic.Value; }
        }
        public ProjectRoot Project { get { return _topic.Value == null ? null : _topic.Value.Project; } }

        public string File
        {
            get { return _topic.Value == null ? null : _topic.Value.File.FilePath; }
        }
    }
}