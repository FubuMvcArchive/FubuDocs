using System;
using FubuCore;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class download_an_asset_file
    {
        private FileSystem theFileSystem;
        private string theDirectory;
        private DownloadToken theToken;
        private string theContents;
        private DownloadContext theContext;
        private DownloadAsset theStep;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = new FileSystem();
            theDirectory = Guid.NewGuid().ToString();
            theFileSystem.CreateDirectory(theDirectory);

            theContext = DownloadContext.For(theDirectory, "http://localhost:5500", new StubPageSource());
            theToken = DownloadToken.For("http://localhost:5500", "/_content/styles/default.css");
            theContents = "body { color: #000; }";

            DownloadScenario.Create(scenario => scenario.Url(theToken.Url, theContents));

            theStep = new DownloadAsset(theToken);
            theStep.Execute(theContext);
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
                .ReadStringFromFile(theDirectory, "_content", "styles", "default.css")
                .ShouldEqual(theContents);
        }

        [Test]
        public void reports_the_download_completed()
        {
            theContext.Report.ShouldHaveTheSameElementsAs(new ItemDownloaded(theToken, theContext.Plan.OutputDirectory, theToken.GetLocalPath(theDirectory)));
        }
    }
}