using System.Linq;
using System.Collections.Generic;
using FubuDocs.Topics;
using FubuMVC.Core.Http;
using FubuCore;

namespace FubuDocs
{
    // TODO -- this should be in FubuMVC itself
    public static class CurrentHttpRequestExtensions
    {
        public static string ToRelativeUrl(this ICurrentHttpRequest request, string url)
        {
            if (request.RelativeUrl().IsEmpty()) return url;

            var current = request.RelativeUrl().TrimStart('/');
            var contentUrl = url.TrimStart('/');

            if (current == string.Empty)
            {
                return contentUrl;
            }

            var prepend = current.Split('/').Select(x => "..").Join("/");
            var relativeUrl = prepend.AppendUrl(contentUrl);

            return relativeUrl;
        }
    }
}