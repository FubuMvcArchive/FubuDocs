using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class TopicIntegratedTester
    {
        [Test]
        public void determines_the_key()
        {
            ObjectMother.Topics["fubumvc/colors/red"].ShouldNotBeNull();
            ObjectMother.Topics["fubumvc/colors/blue"].ShouldNotBeNull();
            ObjectMother.Topics["fubumvc/colors/purple"].ShouldNotBeNull();
            ObjectMother.Topics["fubumvc/colors/green"].ShouldNotBeNull();
        }

        [Test]
        public void strips_the_order_out_of_folder_names()
        {
            ObjectMother.Topics["fubumvc/deep/b"].Url.ShouldEqual("fubumvc/deep/b");
        }

        [Test]
        public void strips_the_order_out_of_deep_folder_names()
        {
            ObjectMother.Topics["fubumvc/deep/b/subjectA/C"]
                .Url.ShouldEqual("fubumvc/deep/b/subjecta/c");
        }


        [Test]
        public void determine_the_url_for_an_index_name()
        {
            var colorsIndex = ObjectMother.Topics["fubumvc/colors"];
            colorsIndex.File.Name.ShouldEqual("index");

            colorsIndex.Url.ShouldEqual("fubumvc/colors");
        }

        [Test]
        public void determine_the_url_for_a_file_not_the_index()
        {
            ObjectMother.Topics["fubumvc/colors/red"].Url.ShouldEqual("fubumvc/colors/red");
        }

        [Test]
        public void determine_the_url_for_a_file_overriding_url_in_spark_file()
        {
            ObjectMother.Topics["fubumvc/colors/green"].Url.ShouldEqual("fubumvc/colors/seagreen"); // look at the 1.1.2.green.spark file
        }


        [Test]
        public void get_the_title_if_it_is_not_written_into_the_file()
        {
            ObjectMother.Topics["fubumvc/colors/green"].Title.ShouldEqual("The green page");
        }

        [Test]
        public void get_the_title_from_file_contents()
        {
            ObjectMother.Topics["fubumvc/colors/blue"].Title.ShouldEqual("Blue");
        }

    }
}