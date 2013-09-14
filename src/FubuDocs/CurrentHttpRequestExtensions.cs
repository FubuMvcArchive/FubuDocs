using System.IO;
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
        public static string ToRelativeUrl(this ICurrentHttpRequest request, FubuDocsDirectories directories, string url)
        {
            var relativeUrl = directories.RootUrls ? url : request.ToRelativeUrl(url);

            if (directories.UseIndexHtml && !Path.HasExtension(relativeUrl))
            {
                relativeUrl = relativeUrl.AppendUrl("index.html");
            }

            return relativeUrl;
        }

        public static string ToRelativeUrl(this ICurrentHttpRequest request, string url)
        {
            var requestUrl = request.RelativeUrl();
            if (requestUrl.IsEmpty() || requestUrl == "/") return url.TrimStart('/');

            var current = requestUrl.TrimStart('/');
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