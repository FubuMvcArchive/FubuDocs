namespace FubuDocs.Topics
{
    public interface ITopicNode
    {
        string Url { get; }
        ProjectRoot Project { get; }
    }
}