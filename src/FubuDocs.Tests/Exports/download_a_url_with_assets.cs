using System;
using FubuCore;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class download_a_url_with_assets
    {
        private FileSystem theFileSystem;
        private string theDirectory;
        private string theContents;
        private DownloadToken theToken;
        private DownloadContext theContext;
        private DownloadUrl theStep;
        private StubPageSource theSource;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = new FileSystem();
            theDirectory = Guid.NewGuid().ToString();
            theFileSystem.CreateDirectory(theDirectory);

            theToken = DownloadToken.For("http://localhost:5500", "/hello");

            theSource = new StubPageSource();
            theContents = buildContents().ToString();
            theSource.Store(theToken, theContents);

            theContext = DownloadContext.For(theDirectory, "http://localhost:5500", theSource);

            theStep = new DownloadUrl(theToken, theSource);
            theStep.Execute(theContext);
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
            theFileSystem.DeleteDirectory(theDirectory);

            DownloadManager.Live();
        }

        [Test]
        public void verify_the_downloaded_file()
        {
            new FileSystem()
                .ReadStringFromFile(theDirectory, "hello", "index.html")
                .ShouldEqual(theContents);
        }

        [Test]
        public void reports_the_download_completed()
        {
            theContext.Report.ShouldHaveTheSameElementsAs(new ItemDownloaded(theToken, theContext.Plan.OutputDirectory, theToken.GetLocalPath(theDirectory)));
        }

        [Test]
        public void queues_up_the_asset_downloads()
        {
            theContext.Plan.Steps
                .ShouldHaveTheSameElementKeysAs(new []
                    {
                        "http://localhost:5500/_content/images/fav.ico",
                        "http://localhost:5500/_content/styles/resets.css",
                        "http://localhost:5500/_content/styles/default.css",
                        "http://localhost:5500/_content/images/logo.png",
                        "http://localhost:5500/_content/scripts/lib/jquery.min.js",
                        "http://localhost:5500/_content/scripts/core.min.js"
                    }, x => x.Token.Url);
        }
    }
}