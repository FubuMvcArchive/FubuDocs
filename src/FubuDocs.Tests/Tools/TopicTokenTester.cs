using FubuDocs.Tools;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class TopicTokenTester
    {
        [Test]
        public void add_child()
        {
            var parent = new TopicToken();
            var child1 = new TopicToken();
            var child2 = new TopicToken();
            var child3 = new TopicToken();
            var child4 = new TopicToken();
        
            parent.AddChild(child1);
            parent.AddChild(child2);
            parent.AddChild(child3);
            parent.AddChild(child4);

            child1.Order.ShouldEqual(1);
            child2.Order.ShouldEqual(2);
            child3.Order.ShouldEqual(3);
            child4.Order.ShouldEqual(4);
        }
    }
}