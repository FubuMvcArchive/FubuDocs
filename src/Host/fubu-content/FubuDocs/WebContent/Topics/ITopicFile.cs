using FubuMVC.Core.View;

namespace FubuDocs.Topics
{
    public interface ITopicFile
    {
        string FilePath { get; }
        string Name { get; }
        string Folder { get; }
        IViewToken ToViewToken();
    }
}