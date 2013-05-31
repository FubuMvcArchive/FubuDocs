using System.Linq;
using FubuDocsRunner.Exports;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class CssUrlStrategyTester
    {
        [Test]
        public void finds_the_link_tags()
        {
            var strategy = new CssUrlStrategy();
            strategy
                .TokensFor(DownloadToken.For("http://localhost", "http://localhost/_content/styles/default.css"), theCssDocument)
                .Select(x => x.Url)
                .ShouldHaveTheSameElementsAs(
                    "http://localhost/_content/img/root.png",    
                    "http://localhost/_content/styles/img/nested.png"    
                );
        }

        [Test]
        public void finds_the_link_tags_from_a_nested_directory()
        {
            var strategy = new CssUrlStrategy();
            strategy
                .TokensFor(DownloadToken.For("http://localhost", "http://localhost/_content/styles/nested/core.css"), theCssDocument)
                .Select(x => x.Url)
                .ShouldHaveTheSameElementsAs(
                    "http://localhost/_content/styles/img/root.png",    
                    "http://localhost/_content/styles/nested/img/nested.png"  
                );
        }

        [Test]
        public void relative_path()
        {
            CssUrlStrategy
                .BuildRelativePath("/_content/styles/default.css", "../images/root.png")
                .ShouldEqual("/_content/images/root.png");
        }

        [Test]
        public void relative_nested_path()
        {
            CssUrlStrategy
                .BuildRelativePath("/_content/styles/nested/default.css", "../images/root.png")
                .ShouldEqual("/_content/styles/images/root.png");
        }

        [Test]
        public void same_path()
        {
            CssUrlStrategy
                .BuildRelativePath("/_content/styles/nested/default.css", "./images/root.png")
                .ShouldEqual("/_content/styles/nested/images/root.png");
        }

        private string theCssDocument
        {
            get 
            { 
                return @".jumbotron:after {
                  content: '';
                  display: block;
                  position: absolute;
                  top: 0;
                  right: 0;
                  bottom: 0;
                  left: 0;
                  background: url(../img/root.png) repeat center center;
                  something-else: url(img/nested.png);
                  opacity: .4;
                }"; 
            }
        }
    }
}