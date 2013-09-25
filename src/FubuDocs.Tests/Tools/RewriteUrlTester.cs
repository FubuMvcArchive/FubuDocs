using FubuCore;
using FubuDocs.Tools;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class RewriteUrlTester
    {
        [Test]
        public void write_url_when_it_does_not_already_exist()
        {
            new FileSystem().WriteStringToFile("foo.txt", @"
<markdown>

</markdown>
");

            new RewriteUrl("foo.txt", "foo").Execute();

            new FileSystem().ReadStringFromFile("foo.txt")
                .ShouldContain("<!-- Url: foo -->");
        }

        [Test]
        public void replace_existing_url()
        {
            new FileSystem().WriteStringToFile("foo.txt", @"
<!-- Url: bar -->
<markdown>

</markdown>
");

            new RewriteUrl("foo.txt", "foo").Execute();

            var resultingText = new FileSystem().ReadStringFromFile("foo.txt");
            resultingText
                .ShouldContain("<!-- Url: foo -->");

            resultingText.ShouldNotContain("<!-- Url: bar -->");
        }
    }
}