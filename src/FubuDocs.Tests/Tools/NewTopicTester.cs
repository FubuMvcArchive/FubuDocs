using FubuDocs.Tools;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class NewTopicTester
    {
        private TopicToken theToken;

        [SetUp]
        public void SetUp()
        {
            TopicToken.FileSystem.DeleteFile("foo.spark");

            theToken = new TopicToken
            {
                Title = "The Foo",
                Url = "foo",
                File = "foo.spark"
            };

            new NewTopic(theToken).Execute();
        }

        [Test]
        public void writes_the_file()
        {
            TopicToken.FileSystem.FileExists("foo.spark")
                .ShouldBeTrue();
        }

        [Test]
        public void should_write_the_title()
        {
            TopicToken.FileSystem.ReadStringFromFile("foo.spark")
                .ShouldContain("<!--Title: The Foo-->");
        }

        [Test]
        public void should_write_the_url_since_it_is_non_null()
        {
            TopicToken.FileSystem.ReadStringFromFile("foo.spark")
                .ShouldContain("<!--Url: foo-->");
        }

        [Test]
        public void adds_a_TODO()
        {
            TopicToken.FileSystem.ReadStringFromFile("foo.spark")
                .ShouldContain("TODO(Write some content!)");
        }
    }
}