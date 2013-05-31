namespace FubuDocsRunner.Exports
{
    public interface IDownloadStep
    {
        DownloadToken Token { get; }
        void Execute(DownloadContext context);
    }
}