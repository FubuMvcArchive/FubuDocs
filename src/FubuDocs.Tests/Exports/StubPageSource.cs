using FubuCore.Util;
using FubuDocsRunner.Exports;

namespace FubuDocs.Tests.Exports
{
    public class StubPageSource : IPageSource
    {
        private readonly Cache<DownloadToken, string> _sources = new Cache<DownloadToken, string>();

        public void Store(DownloadToken token, string source)
        {
            _sources.Fill(token, source);
        }

        public string SourceFor(DownloadToken token)
        {
            return _sources[token];
        }
    }
}