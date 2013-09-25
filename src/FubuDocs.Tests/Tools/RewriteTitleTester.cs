using FubuCore;
using FubuDocs.Tools;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class RewriteTitleTester
    {
        [Test]
        public void write_title_when_it_does_not_already_exist()
        {
            new FileSystem().WriteStringToFile("foo.txt", @"
<markdown>

</markdown>
");

            new RewriteTitle("foo.txt", "foo").Execute();

            new FileSystem().ReadStringFromFile("foo.txt")
                .ShouldContain("<!-- Title: foo -->");
        }

        [Test]
        public void replace_existing_title()
        {
            new FileSystem().WriteStringToFile("foo.txt", @"
<!-- Title: bar -->
<markdown>

</markdown>
");

            new RewriteTitle("foo.txt", "foo").Execute();

            var resultingText = new FileSystem().ReadStringFromFile("foo.txt");
            resultingText
                .ShouldContain("<!-- Title: foo -->");

            resultingText.ShouldNotContain("<!-- Title: bar -->");
        }
    }
}