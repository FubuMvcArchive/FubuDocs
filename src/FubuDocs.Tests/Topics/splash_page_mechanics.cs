using FubuDocs.Topics;
using FubuMVC.Core.Behaviors.Chrome;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class splash_page_mechanics
    {
        private Topic splashTopic;
        private ProjectRoot fubudocs;

        [SetUp]
        public void SetUp()
        {
            fubudocs = ObjectMother.TopicGraph.ProjectFor("FubuDocs");
            fubudocs.ShouldNotBeNull();

            splashTopic = fubudocs.Splash;
        }

        [Test]
        public void splash_exists()
        {
            splashTopic.ShouldNotBeNull();
        }

        [Test]
        public void splash_is_not_the_same_as_the_index()
        {
            splashTopic.ShouldNotBeTheSameAs(fubudocs.Index);
        }

        [Test]
        public void splash_should_have_The_project_name_as_url()
        {
            splashTopic.Url.ShouldEqual("fubudocs");
        }

        [Test]
        public void the_index_now_has_index_in_its_url()
        {
            fubudocs.Index.Url.ShouldEqual("fubudocs/index");
        }

        [Test]
        public void splash_chain_uses_the_splash_chrome()
        {
            var chain = splashTopic.BuildChain();

            chain.OfType<ChromeNode>().Single()
                 .Title().ShouldEqual(fubudocs.Splash.Title);

            chain.OfType<ChromeNode>().Single().
                  ContentType.ShouldEqual(typeof (SplashChrome));
        }

        [Test]
        public void splash_is_not_in_all_topics_for_the_project()
        {
            fubudocs.AllTopics().ShouldNotContain(splashTopic);
        }

        [Test]
        public void the_splash_topic_and_the_index_are_both_in_the_behavior_graph()
        {
            ObjectMother.Behaviors.Behaviors.FirstOrDefault(x => x.GetRoutePattern() == "fubudocs").ShouldNotBeNull();
            ObjectMother.Behaviors.Behaviors.FirstOrDefault(x => x.GetRoutePattern() == "fubudocs/index").ShouldNotBeNull();
        }
        
    }
}