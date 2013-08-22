using System;
using System.Collections.Generic;
using FubuDocsRunner.Running;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using HtmlTags;
using FubuCore;
using System.Linq;

namespace FubuDocsRunner.Exports
{
    public interface IAccessedCache
    {
        void Enqueue(string url);
        IEnumerable<ContentUrl> Dequeue();

        bool Any();
    }

    public class AccessedCache : IAccessedCache
    {
        private readonly IList<string> _visited = new List<string>(); 
        private readonly List<ContentUrl> _queued = new List<ContentUrl>();
 
        public void Enqueue(string url)
        {
            if (_visited.Contains(url)) return;

            _queued.Add(new ContentUrl(url));

            _visited.Add(url);
        }

        public IEnumerable<ContentUrl> Dequeue()
        {
            _queued.Sort();
            var returnValue = _queued.ToArray();

            _queued.Clear();

            return returnValue;
        }

        public bool Any()
        {
            return _queued.Any();
        }
    }

    public class ExportAssetTagWriter : IAssetTagWriter
    {
        private readonly AssetTagWriter _inner;
        private readonly IAccessedCache _cache;
        private readonly FubuDocsDirectories _directories;

        public ExportAssetTagWriter(AssetTagWriter inner, IAccessedCache cache, FubuDocsDirectories directories)
        {
            _inner = inner;
            _cache = cache;
            _directories = directories;
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
            var relativeUrl = _directories.CorrectUrl(rawUrl);

            _cache.Enqueue(relativeUrl);

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

    public class ExportAssetUrls : IAssetUrls
    {
        private readonly AssetUrls _inner;
        private readonly IAccessedCache _cache;
        private readonly FubuDocsDirectories _directories;

        public ExportAssetUrls(AssetUrls inner, IAccessedCache cache, FubuDocsDirectories directories)
        {
            _inner = inner;
            _cache = cache;
            _directories = directories;
        }

        public string UrlForAsset(AssetFolder folder, string name)
        {
            var url = _inner.UrlForAsset(folder, name);

            _cache.Enqueue(url);

            return _directories.CorrectUrl(url);
        }
    }
}