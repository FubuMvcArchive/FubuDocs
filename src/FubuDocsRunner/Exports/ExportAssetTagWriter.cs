using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FubuDocsRunner.Running;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
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
            if (url.Contains("_fubu")) return;

            url = url.TrimStart('/');
            while (url.StartsWith("../"))
            {
                url = url.TrimStart('.', '.', '/');
            }

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

            return _directories.ToRelativeContentUrl(url, _request);
        }
    }

    public class ExportContentWriter : IContentWriter
    {
        private readonly ContentWriter _writer;
        private readonly IAccessedCache _cache;
        private readonly ICurrentHttpRequest _request;

        public ExportContentWriter(ContentWriter writer, IAccessedCache cache, ICurrentHttpRequest request)
        {
            _writer = writer;
            _cache = cache;
            _request = request;
        }

        public bool Write(AssetPath asset, Action<IEnumerable<AssetFile>> writeHeaders)
        {
            _cache.Enqueue(_request.RelativeUrl());
            return _writer.Write(asset, writeHeaders);
        }
    }

    public class ExportUrlRegistry : IUrlRegistry
    {
        private readonly UrlRegistry _inner;
        private readonly IAccessedCache _cache;

        public ExportUrlRegistry(UrlRegistry inner, IAccessedCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public string UrlFor(object model, string categoryOrHttpMethod = null)
        {
            var url = _inner.UrlFor(model, categoryOrHttpMethod);
            _cache.Enqueue(url);

            return url;
        }

        public string UrlFor<T>(string categoryOrHttpMethod = null) where T : class
        {
            var url = _inner.UrlFor<T>(categoryOrHttpMethod);
            _cache.Enqueue(url);

            return url;
        }

        public string UrlFor(Type handlerType, MethodInfo method = null, string categoryOrHttpMethodOrHttpMethod = null)
        {
            var url = _inner.UrlFor(handlerType, method, categoryOrHttpMethodOrHttpMethod);
            _cache.Enqueue(url);

            return url;
        }

        public string UrlFor<TController>(Expression<Action<TController>> expression, string categoryOrHttpMethod = null)
        {
            var url = _inner.UrlFor(expression, categoryOrHttpMethod);
            _cache.Enqueue(url);

            return url;
        }

        public string UrlForNew(Type entityType)
        {
            return _inner.UrlForNew(entityType);
        }

        public bool HasNewUrl(Type type)
        {
            return _inner.HasNewUrl(type);
        }

        public string TemplateFor(object model, string categoryOrHttpMethod = null)
        {
            return _inner.TemplateFor(model, categoryOrHttpMethod);
        }

        public string TemplateFor<TModel>(params Func<object, object>[] hash) where TModel : class, new()
        {
            return _inner.TemplateFor<TModel>(hash);
        }

        public string UrlFor(Type modelType, RouteParameters parameters, string categoryOrHttpMethod = null)
        {
            var url = _inner.UrlFor(modelType, parameters, categoryOrHttpMethod);
            _cache.Enqueue(url);

            return url;
        }
    }
}