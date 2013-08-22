using System.Linq;
using System.Collections.Generic;

namespace FubuDocs.Topics
{
    public static class StringExtensions
    {
         public static string AppendUrl(this string url, string part)
         {
             var composite = (url ?? string.Empty).TrimEnd('/') + "/" + part.TrimStart('/');

             return composite.TrimEnd('/');
         }

        public static string ChildUrl(this string url)
        {
            return url.Split('/').Skip(1).Join("/");
        }

        public static string ParentUrl(this string url)
        {
            url = url.Trim('/');
            return url.Contains("/") ? url.Split('/').Reverse().Skip(1).Reverse().Join("/") : string.Empty;
        }
    }
}