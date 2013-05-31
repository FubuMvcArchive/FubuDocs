using FubuCore;
using FubuDocs.Topics;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class SparkTopicFileTester
    {
        private ViewDescriptor<Template> theDescriptor;
        private SparkTopicFile theTopicFile;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().WriteStringToFile("test.spark", @"




");

            theDescriptor = new ViewDescriptor<Template>(new Template("test.spark".ToFullPath(), "folder1/folder2/test.spark", "bottle1"));

            theTopicFile = new SparkTopicFile(theDescriptor);
        }

        [Test]
        public void Name_from_the_descriptor()
        {
            theTopicFile.Name.ShouldEqual("test");
        }

        [Test]
        public void view_path_from_the_descriptor()
        {
            theTopicFile.FilePath.ShouldEqual("test.spark".ToFullPath());
        }

        [Test, Ignore("Dunno how to make this work yet.  Think it's going to take an integration test to pull it off")]
        public void relative_path_from_the_descriptor()
        {
            Assert.Fail("TODO");
        }


    }
}