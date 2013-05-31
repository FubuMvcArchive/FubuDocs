using System.Collections.Generic;
using System.Linq;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class DownloadTokenParserTester
    {
        [Test]
        public void aggregates_the_strategies()
        {
            var s1 = new StubStrategy("/a1", "/a2");
            var s2 = new StubStrategy("/a3", "/a4");

            DownloadTokenParser.Clear();
            DownloadTokenParser.AddStrategy(s1);
            DownloadTokenParser.AddStrategy(s2);

            DownloadTokenParser.TokensFor(DownloadToken.For("http://localhost", ""), "test").Select(x => x.Url).ShouldHaveTheSameElementsAs("http://localhost/a1", "http://localhost/a2", "http://localhost/a3", "http://localhost/a4");

            DownloadTokenParser.Reset();
        }



        public class StubStrategy : IDownloadTokenStrategy
        {
            private readonly IEnumerable<DownloadToken> _assets;

            public StubStrategy(params string[] assets)
            {
                _assets = assets.Select(x => DownloadToken.For("http://localhost", x));
            }

            public IEnumerable<DownloadToken> TokensFor(DownloadToken token, string source)
            {
                return _assets;
            }
        }
    }
}