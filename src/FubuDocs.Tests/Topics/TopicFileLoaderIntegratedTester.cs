using System.Linq;
using FubuDocs.Topics;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class TopicFileLoaderIntegratedTester
    {
        [Test]
        public void ignores_any_file_in_examples()
        {
            ObjectMother.Files.Any(x => x.Name == "anything").ShouldBeFalse();
        }

        [Test]
        public void ignores_any_file_in_samples()
        {
            ObjectMother.Files.Any(x => x.Name == "whatever").ShouldBeFalse();
        }

        [Test]
        public void should_find_files()
        {
            ObjectMother.Files.Any().ShouldBeTrue();
        }

        [Test]
        public void spot_check_a_topic()
        {
            ITopicFile file = ObjectMother.Files.First(x => x.Name == "1.3.purple");
            file.ShouldNotBeNull();

            file.Folder.ShouldEqual("colors");
        }
    }
}