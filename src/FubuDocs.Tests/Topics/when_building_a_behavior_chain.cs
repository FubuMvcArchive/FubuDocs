using FubuDocs.Topics;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using FubuCore;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class when_building_a_behavior_chain
    {
        private StubTopicFile theFile;
        private Topic theTopic;
        private BehaviorChain theChain;

        [SetUp]
        public void SetUp()
        {
            theTopic = ObjectMother.Topics["fubumvc/colors/red"];
            theChain = theTopic.BuildChain();
        }

        [Test]
        public void chain_should_have_the_url()
        {
            theChain.Route.Pattern.ShouldEqual(theTopic.Url);
        }

        [Test]
        public void uses_the_key_for_url_category()
        {
            theChain.UrlCategory.Category.ShouldEqual(theTopic.Key);
        }

        [Test]
        public void should_have_a_node_for_the_topic()
        {
            var node = theChain.OfType<TopicBehaviorNode>().Single();
            node.ShouldBeOfType<IMayHaveInputType>().InputType().ShouldEqual(typeof (Topic));
            node.Topic.ShouldBeTheSameAs(theTopic);
        }

        [Test]
        public void should_have_the_view_token_from_the_file()
        {
            var node = theChain.OfType<TopicBehaviorNode>().Single();
            node.View.View.Name().ShouldEqual(theTopic.File.Name);
        }

        [Test]
        public void the_chain_can_build_its_object_def_smoke_test()
        {
            theChain.As<IContainerModel>().ToObjectDef().ShouldNotBeNull();
        }
    }
}