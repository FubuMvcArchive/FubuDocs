using System.Collections.Generic;
using System.Linq;

namespace FubuDocsRunner.Exports
{
    public class DownloadTokenParser
    {
        private static readonly IList<IDownloadTokenStrategy> Strategies = new List<IDownloadTokenStrategy>();
 
        static DownloadTokenParser()
        {
            Reset();
        }

        public static void Reset()
        {
            Clear();

            AddStrategy<LinkTagStrategy>();
            AddStrategy<ImgTagStrategy>();
            AddStrategy<ScriptTagStrategy>();
            AddStrategy<AnchorTagStrategy>();
            AddStrategy<CssUrlStrategy>();
        }

        public static void Clear()
        {
            Strategies.Clear();
        }

        public static void AddStrategy<T>() where T : IDownloadTokenStrategy, new()
        {
            AddStrategy(new T());
        }

        public static void AddStrategy(IDownloadTokenStrategy strategy)
        {
            Strategies.Add(strategy);
        }

        public static IEnumerable<DownloadToken> TokensFor(DownloadToken token, string source)
        {
            return Strategies.SelectMany(strategy => strategy.TokensFor(token, source));
        }
    }
}