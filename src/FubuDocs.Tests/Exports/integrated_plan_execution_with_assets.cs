using System.Collections.Generic;
using FubuCore;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class integrated_plan_execution_with_assets
    {
        private DownloadScenario theScenario;
        private string theContents;
        private DownloadToken theToken;
        private DownloadPlan thePlan;
        private StubPageSource theSource;

        [SetUp]
        public void SetUp()
        {
            theScenario = DownloadScenario.Create(scenario =>
            {
                scenario.Url("http://localhost:5500/_content/images/fav.ico", "fav");
                scenario.Url("http://localhost:5500/_content/styles/resets.css", "resets");
                scenario.Url("http://localhost:5500/_content/styles/default.css", "default");

                scenario.Url("http://localhost:5500/_content/images/logo.png", "logo");

                scenario.Url("http://localhost:5500/_content/scripts/lib/jquery.min.js", "jquery");
                scenario.Url("http://localhost:5500/_content/scripts/core.min.js", "core");
            });

            theContents = buildContents().ToString();
            theToken = DownloadToken.For("http://localhost:5500", "/hello");

            theSource = new StubPageSource();
            theSource.Store(theToken, theContents);

            theSource.Store(DownloadToken.For("http://localhost:5500", "/_content/styles/resets.css"), "resets");
            theSource.Store(DownloadToken.For("http://localhost:5500", "/_content/styles/default.css"), "default");

            thePlan = new DownloadPlan(theScenario.Directory, "http://localhost:5500", theSource);

            thePlan.Add(new DownloadUrl(theToken, theSource));

            

            thePlan.Execute();
        }

        private HtmlDocument buildContents()
        {
            var document = new HtmlDocument();
            document.Head.Add("link", link => link.Attr("href", "/_content/images/fav.ico"));
            document.Head.Add("link", link => link.Attr("href", "/_content/styles/resets.css"));
            document.Head.Add("link", link => link.Attr("href", "/_content/styles/default.css"));

            document.Body.Add("h1", h1 => h1.Text("Hello World"));
            document.Body.Add("img", img => img.Attr("src", "/_content/images/logo.png"));

            document.Body.Add("script", script => script.Attr("src", "/_content/scripts/lib/jquery.min.js"));
            document.Body.Add("script", script => script.Attr("src", "/_content/scripts/core.min.js"));

            return document;
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void downloads_topic_AND_assets()
        {
            fileContentsFor("hello", "index.html").ShouldEqual(theContents);

            verifyAsset("http://localhost:5500/_content/images/fav.ico", "_content", "images", "fav.ico");

            verifyAsset("http://localhost:5500/_content/styles/resets.css", "_content", "styles", "resets.css");
            verifyAsset("http://localhost:5500/_content/styles/default.css", "_content", "styles", "default.css");

            verifyAsset("http://localhost:5500/_content/images/logo.png", "_content", "images", "logo.png");

            verifyAsset("http://localhost:5500/_content/scripts/lib/jquery.min.js", "_content", "scripts", "lib", "jquery.min.js");
            verifyAsset("http://localhost:5500/_content/scripts/core.min.js", "_content", "scripts", "core.min.js");
        }

        public string fileContentsFor(params string[] parts)
        {
            var fileSystem = new FileSystem();

            var path = new List<string>();
            path.Add(theScenario.Directory);
            path.AddRange(parts);

            return fileSystem
                .ReadStringFromFile(path.ToArray());
        }

        private void verifyAsset(string url, params string[] parts)
        {
            var expectedContents = theScenario.ContentsFor(url);
            fileContentsFor(parts).ShouldEqual(expectedContents);
        }
    }
}