using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;

namespace FubuDocsRunner.Exports
{
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