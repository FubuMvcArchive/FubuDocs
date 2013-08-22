using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuDocsRunner.Exports
{
    public interface IDownloadTokenStrategy
    {
        IEnumerable<DownloadToken> TokensFor(DownloadToken token, string source);
    }

    public abstract class DownloadTokenStrategy : IDownloadTokenStrategy
    {
        private readonly Regex _regex;
        private readonly string _attribute;

        protected DownloadTokenStrategy(string expression, string attribute)
        {
            _regex = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            _attribute = attribute;
        }

        public IEnumerable<DownloadToken> TokensFor(DownloadToken token, string source)
        {
            var list = new List<DownloadToken>();

            var matches = _regex.Matches(source);
            foreach (Match match in matches)
            {
                var value = HtmlElement.GetAttributeValue(match.Value, _attribute);
                if (value.IsNotEmpty() && value.StartsWith("/"))
                {
                    try
                    {
                        list.Add(DownloadToken.For(token.BaseUrl, value));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in parsing a page for links");
                        Console.WriteLine(source);
                        throw new MalformedDownloadTokenException(token, e);
                    }
                }
            }

            return list;
        }
    }

    public class LinkTagStrategy : DownloadTokenStrategy
    {
        public LinkTagStrategy()
            : base("<link\\b[^>]*>", "href")
        {
        }
    }

    public class ImgTagStrategy : DownloadTokenStrategy
    {
        public ImgTagStrategy()
            : base("<img\\b[^>]*>", "src")
        {
        }
    }

    public class ScriptTagStrategy : DownloadTokenStrategy
    {
        public ScriptTagStrategy()
            : base("<script\\b[^>]*>", "src")
        {
        }
    }

    public class AnchorTagStrategy : DownloadTokenStrategy
    {
        public AnchorTagStrategy()
            : base("<a\\b[^>]*>", "href")
        {
        }
    }

    public class CssUrlStrategy : IDownloadTokenStrategy
    {
        private readonly static Regex Expression = new Regex("url\\(['\"]?([^)]+?)['\"]?\\)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

        public IEnumerable<DownloadToken> TokensFor(DownloadToken token, string source)
        {
            var list = new List<DownloadToken>();
            var matches = Expression.Matches(source);
            foreach (Match match in matches)
            {
                var value = match.Value;
                if (value.IsNotEmpty())
                {
                    value = value.Replace("url", "").Replace("\"", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim().Replace(token.BaseUrl, "");
                    var relative = BuildRelativePath(token.RelativeUrl, value);

                    if (relative.Contains("<span>") || relative.Contains("</span>"))
                    {
                        Console.WriteLine("Ignoring link " + relative);
                        continue;
                    }

                    try
                    {
                        list.Add(DownloadToken.For(token.BaseUrl, relative));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Ignoring link " + relative);
                    }

                }
            }

            return list;
        }

        public static string BuildRelativePath(string origin, string relativePath)
        {
            var parts = origin.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            var tmp = parts.Take(parts.Length - 1).ToList();

            var originalParts = new Stack<string>(tmp);
            var targetParts = relativePath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            var newParts = new List<string>();
            foreach (var part in targetParts)
            {
                if (part == "..")
                {
                    originalParts.Pop();
                    continue;
                }

                if (part == ".")
                {
                    continue;
                }

                if (part.Contains("data:image"))
                {
                    break;
                }

                newParts.Add(part);
            }

            var relativeParts = new List<string>(originalParts.Reverse());
            relativeParts.AddRange(newParts);

            var relative = relativeParts.Aggregate("", (current, t) => current + ("/" + t));
            Console.WriteLine(relative);

            return relative;
        }
    }
}