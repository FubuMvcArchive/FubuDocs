using System.Linq;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class ScriptTagStrategyTester
    {
        [Test]
        public void finds_the_script_tags()
        {
            var strategy = new ScriptTagStrategy();
            strategy
                .TokensFor(DownloadToken.For("http://localhost", ""), theDocument.ToString())
                .Select(x => x.Url)
                .ShouldHaveTheSameElementsAs("http://localhost/_content/scripts/lib/jquery.min.js", "http://localhost/_content/scripts/core.min.js");
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