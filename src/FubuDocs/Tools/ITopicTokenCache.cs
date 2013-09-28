namespace FubuDocs.Tools
{
    public interface ITopicTokenCache
    {
        TopicToken TopicStructureFor(string projectName);
        void RewriteTopicStructure(string projectName, TopicToken newStructure);
    }
}