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
                        .Version.ShouldEqual(Assembly.Load("FubuDocs.Docs").GetName().Version.ToString());
        }
    }
}