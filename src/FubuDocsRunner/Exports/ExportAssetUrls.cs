using FubuDocsRunner.Running;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Http;
using FubuDocs;

namespace FubuDocsRunner.Exports
{
    public class ExportAssetUrls : IAssetUrls
    {
        private readonly AssetUrls _inner;
        private readonly IAccessedCache _cache;
        private readonly FubuDocsDirectories _directories;
        private readonly ICurrentHttpRequest _request;

        public ExportAssetUrls(AssetUrls inner, IAccessedCache cache, FubuDocsDirectories directories, ICurrentHttpRequest request)
        {
            _inner = inner;
            _cache = cache;
            _directories = directories;
            _request = request;
        }

        public string UrlForAsset(AssetFolder folder, string name)
        {
            var url = _inner.UrlForAsset(folder, name);

            _cache.Enqueue(url);

            return _request.ToRelativeUrl(_directories, url);
        }
    }
}