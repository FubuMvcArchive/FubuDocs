using System.Collections.Generic;
using FubuDocs.Topics;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class TopicFolderTester
    {
        private TopicFolder theFolder;

        private IEnumerable<ITopicFile> addFiles(params string[] names)
        {
            var files = names.Select(x => new StubTopicFile {Name = x, FilePath = x + ".spark"}).Select(x => new Topic(new ProjectRoot(), x)).ToArray();
            theFolder.AddFiles(files);

            return files.Select(x => x.File).ToArray();
        }

        [SetUp]
        public void SetUp()
        {
            theFolder = new TopicFolder("1.foo/1.bar/2.abc", new ProjectRoot { Url = "fubumvc" });
        }

        [Test]
        public void captures_the_raw_name()
        {
            theFolder.OrderString.ShouldEqual("1.foo/1.bar/2.abc");
        }

        [Test]
        public void uses_only_the_non_ordered_name_of_the_last_item()
        {
            theFolder.Name.ShouldEqual("abc");
        }

        [Test]
        public void has_the_url()
        {
            theFolder.Url.ShouldEqual("fubumvc/foo/bar/abc");
        }

        [Test]
        public void organizing_by_index_only()
        {
            var file = addFiles("index").Single();

            theFolder.TopLevelTopics().Single().File.ShouldBeTheSameAs(file);
        }

        [Test]
        public void named_files_only()
        {
            addFiles("b", "c", "a");

            theFolder.TopLevelTopics().Select(x => x.File.Name)
                .ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void index_and_named_files()
        {
            addFiles("b", "c", "a", "index");

            var root = theFolder.TopLevelTopics().Single();
            root.File.Name.ShouldEqual("index");
            root.FirstChild.File.Name.ShouldEqual("a");
            root.FirstChild.NextSibling.File.Name.ShouldEqual("b");
            root.FirstChild.NextSibling.NextSibling.File.Name.ShouldEqual("c");
        }

        [Test]
        public void respect_the_numbering()
        {
            addFiles("1.c", "2.a", "3.b");

            theFolder.TopLevelTopics().Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("c", "a", "b");
        }

        [Test]
        public void respect_the_numbering_with_index_too()
        {
            addFiles("1.b", "3.c", "2.a", "index");

            var root = theFolder.TopLevelTopics().Single();
            root.File.Name.ShouldEqual("index");
            root.FirstChild.File.Name.ShouldEqual("1.b");
            root.FirstChild.NextSibling.File.Name.ShouldEqual("2.a");
            root.FirstChild.NextSibling.NextSibling.File.Name.ShouldEqual("3.c");
        }
    }
}