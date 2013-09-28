using System;
using System.Linq;
using FubuCore;
using FubuDocs.Tools;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class TopicTokenTester
    {
        private TopicToken root;

        [SetUp]
        public void SetUp()
        {
            var directory = ".".ToFullPath().ParentDirectory().ParentDirectory()
                .ParentDirectory()
                .AppendPath("Sample.Docs");

            root = TopicToken.LoadIndex(directory);
        }

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

        [Test]
        public void determine_delta_if_the_topic_is_new()
        {
            var topic = new TopicToken
            {
                Id = Guid.Empty.ToString()
            };

            topic.DetermineDeltas(root)
                .Single().ShouldEqual(new NewTopic(topic));
        }

        [Test]
        public void if_the_path_is_different_return_a_move_content()
        {
            var topic = root.Children.First().Clone();
            var originalFile = topic.File;

            topic.File = Guid.NewGuid().ToString();

            topic.DetermineDeltas(root)
                .Single().ShouldEqual(new MoveTopic(originalFile, topic.File));
        }

        [Test]
        public void if_the_title_is_different_return_rewrite_title()
        {
            var topic = root.Children.First().Clone();
            topic.Title = Guid.NewGuid().ToString();

            topic.DetermineDeltas(root)
                .Single()
                .ShouldEqual(new RewriteTitle(topic.File, topic.Title));
        }

        [Test]
        public void if_the_url_is_different_return_rewrite_url()
        {
            var topic = root.Children.First().Clone();
            topic.Url = Guid.NewGuid().ToString();

            topic.DetermineDeltas(root)
                .Single()
                .ShouldEqual(new RewriteUrl(topic.File, topic.Url));
        }
    }
}