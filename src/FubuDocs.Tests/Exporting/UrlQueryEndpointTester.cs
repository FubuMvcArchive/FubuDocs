using FubuDocs.Exporting;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuDocs.Tests.Exporting
{
    [TestFixture]
    public class UrlQueryEndpointTester
    {
        [Test]
        public void strips_out_the_base_url()
        {
            UrlQueryEndpoint.GetPattern("http://localhost:5500/_fubu").ShouldEqual("/_fubu");
        }

        [Test]
        public void ignores_internal_routes()
        {
            UrlQueryEndpoint.ShouldIgnore("/_fubu").ShouldBeTrue();
        }

        [Test]
        public void ignores_diagnostic_routes()
        {
            UrlQueryEndpoint.ShouldIgnore("/_diagnostics").ShouldBeTrue();
        }

        [Test]
        public void ignores_about()
        {
            UrlQueryEndpoint.ShouldIgnore("/_about").ShouldBeTrue();
        }

        [Test]
        public void allows_others()
        {
            UrlQueryEndpoint.ShouldIgnore("/topics").ShouldBeFalse();
        }
    }
}