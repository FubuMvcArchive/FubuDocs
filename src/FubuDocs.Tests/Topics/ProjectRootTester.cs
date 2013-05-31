using NUnit.Framework;
using FubuTestingSupport;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class ProjectRootTester
    {
        [Test]
        public void can_find_for_the_index()
        {
            ObjectMother.ProjectRoot.FindByKey("fubumvc")
                        .ShouldBeTheSameAs(ObjectMother.ProjectRoot.Index);
        }

        [Test]
        public void can_find_for_deeper_keys()
        {
            ObjectMother.ProjectRoot.FindByKey("fubumvc/colors/red")
                        .ShouldNotBeNull();
        }
    }
}