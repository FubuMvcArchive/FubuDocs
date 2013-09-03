namespace FubuDocsRunner
{
    public interface IStep
    {
        string Description();
        bool Execute();
    }
}