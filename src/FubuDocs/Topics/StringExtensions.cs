using System.Linq;
using System.Collections.Generic;
using FubuCore;

namespace FubuDocs.Topics
{
    public static class StringExtensions
    {
        public static string MoveUp(this string relativeUrl)
        {
            if (relativeUrl.IsEmpty()) return relativeUrl;

            var segments = relativeUrl.Split('/');
            if (segments.Count() == 1) return string.Empty;

            return segments.Skip(1).Join("/");
        }
    }
}