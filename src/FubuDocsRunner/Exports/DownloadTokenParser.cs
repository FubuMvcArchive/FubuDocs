using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

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
            return Strategies.SelectMany(strategy => {
                try
                {
                    return strategy.TokensFor(token, source);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error determining download tokens for '{0}' with {1}", token.RelativeUrl, strategy);

                    throw new MalformedDownloadTokenException(token, ex);
                }
            });
        }

        
    }

    public class MalformedDownloadTokenException : Exception
    {
        private readonly DownloadToken _token;

        public MalformedDownloadTokenException(DownloadToken token, Exception inner) : base("", inner)
        {
            _token = token;
        }

        public override string Message
        {
            get { return "Failed to create download tokens for '{0}'".ToFormat(_token.RelativeUrl); }
        }
    }
}