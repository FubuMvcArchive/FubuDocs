using FubuDocsRunner.Exports;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Exports
{
    [TestFixture]
    public class HtmlElementTester
    {
        [Test]
        public void get_attribute_value()
        {
            var input = "<script src=\"/test\" />";
            HtmlElement.GetAttributeValue(input, "src").ShouldEqual("/test");
        }

        [Test]
        public void get_attribute_value_2()
        {
            var input = "<link type=\"text/css\" rel=\"stylesheet\" href=\"/_content/styles/toastr.css\">";
            HtmlElement.GetAttributeValue(input, "href").ShouldEqual("/_content/styles/toastr.css");
        }

        [Test]
        public void get_attribute_value_from_single_quotes()
        {
            var input = "<a href='/a'>";
            HtmlElement.GetAttributeValue(input, "href").ShouldEqual("/a");
        }
    }
}