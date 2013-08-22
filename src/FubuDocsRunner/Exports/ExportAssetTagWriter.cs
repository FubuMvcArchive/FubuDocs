using System.Collections.Generic;
using FubuDocsRunner.Running;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using HtmlTags;
using FubuCore;

namespace FubuDocsRunner.Exports
{
    public class ExportAssetTagWriter : IAssetTagWriter
    {
        private readonly AssetTagWriter _inner;
        private readonly IAccessedCache _cache;
        private readonly FubuDocsDirectories _directories;
        private readonly ICurrentHttpRequest _request;

        public ExportAssetTagWriter(AssetTagWriter inner, IAccessedCache cache, FubuDocsDirectories directories, ICurrentHttpRequest request)
        {
            _inner = inner;
            _cache = cache;
            _directories = directories;
            _request = request;
        }

        public TagList WriteAllTags()
        {
            var list = _inner.WriteAllTags();

            list.AllTags().Each(correctTag);

            return list;
        }

        private void fixUrl(HtmlTag tag, string attrName)
        {
            var rawUrl = tag.Attr(attrName);
            _cache.Enqueue(rawUrl);
            var relativeUrl = _directories.ToRelativeContentUrl(rawUrl, _request);

            

            tag.Attr(attrName, relativeUrl);
        }

        private void correctTag(HtmlTag tag)
        {
            if (tag.TagName() == "link")
            {
                fixUrl(tag, "href");
            }
            else if (tag.TagName() == "script")
            {
                fixUrl(tag, "src");
            }
        }

        public TagList WriteTags(MimeType mimeType, params string[] tags)
        {
            var list = _inner.WriteTags(mimeType, tags);

            list.AllTags().Each(correctTag);

            return list;
        }
    }
}