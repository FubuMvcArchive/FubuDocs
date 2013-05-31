using System.Linq;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class AnchorTagStrategyTester
    {
        [Test]
        public void finds_the_script_tags()
        {
            var strategy = new AnchorTagStrategy();
            strategy
                .TokensFor(DownloadToken.For("http://localhost", ""), theDocument.ToString())
                .Select(x => x.Url)
                .ShouldHaveTheSameElementsAs("http://localhost/something", "http://localhost/something/else");
        }

        private HtmlDocument theDocument
        {
            get
            {
                var document = new HtmlDocument();
                document.Head.Add("link", link => link.Attr("href", "/_content/images/fav.ico"));
                document.Head.Add("link", link => link.Attr("href", "/_content/styles/resets.css"));
                document.Head.Add("link", link => link.Attr("href", "/_content/styles/default.css"));

                document.Body.Add("h1", h1 => h1.Text("Hello World"));
                document.Body.Add("img", img => img.Attr("src", "/_content/images/logo.png"));

                document.Body.Add("script", script => script.Attr("src", "/_content/scripts/lib/jquery.min.js"));
                document.Body.Add("script", script => script.Attr("src", "/_content/scripts/core.min.js"));

                document.Body.Add("a", a => a.Attr("href", "/something"));
                document.Body.Add("a", a => a.Attr("href", "/something/else"));

                return document;
            }
        }
    }
}