using FubuCore;
using FubuDocs.Topics;
using FubuMVC.Core.Registration.Querying;
using FubuTestingSupport;
using NUnit.Framework;
using System.Collections.Generic;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class can_build_behavior_for_topic_chain
    {
        [Test]
        public void can_find_behavior_chain_for_topic()
        {
            var resolver = new ChainResolutionCache(new TypeResolver(), ObjectMother.Behaviors);
            ObjectMother.ProjectRoot.AllTopics().Each(x => {
                resolver.FindUniqueByType(typeof (Topic), x.Key).ShouldNotBeNull();
            });
        }
    }
}