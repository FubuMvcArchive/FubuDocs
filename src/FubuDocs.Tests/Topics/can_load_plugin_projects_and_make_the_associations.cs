using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class can_load_plugin_projects_and_make_the_associations
    {
        [Test]
        public void project_marked_as_plugin_to_is_attached_to_parent()
        {
            var parent = ObjectMother.TopicGraph.ProjectFor("fubumvc");

            parent.Plugins.Single().ShouldBeTheSameAs(ObjectMother.TopicGraph.ProjectFor("fubumvc.plugin"));
        }
    }
}