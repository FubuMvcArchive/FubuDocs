using System.Collections.Generic;

namespace FubuDocsRunner.Topics
{
    public interface ITopicFileSystem
    {
        IEnumerable<TopicToken> ReadTopics();
        void AddTopic(TopicToken token);
        void Reorder(TopicToken topicToken, int order);
    }
}