using System.Reflection;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class ProjectRoot_version_loading_integration_tester
    {
        [Test]
        public void uses_the_assembly_version()
        {
            ObjectMother.TopicGraph.ProjectFor("FubuDocs")
                        .Version.ShouldEqual("0.9.0"); // this is going to have to change over time
        }
    }
}