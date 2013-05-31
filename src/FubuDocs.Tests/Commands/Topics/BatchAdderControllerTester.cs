using FubuDocsRunner.Topics;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuDocs.Tests.Commands.Topics
{
    [TestFixture]
    public class BatchAdderControllerTester : InteractionContext<BatchAdderController>
    {
        private TopicToken[] _topics;
        private ITopicFileSystem theFileSystem;

        protected override void beforeEach()
        {
            theFileSystem = MockFor<ITopicFileSystem>();
        }

        private void theExistingFilesAre(params string[] names)
        {
            int order = 0;

            _topics = names.Select(x => {
                return new TopicToken {Key = x, Order = ++order};
            }).ToArray();

            MockFor<ITopicFileSystem>().Stub(x => x.ReadTopics())
                                       .Return(_topics);
        }

        [Test]
        public void starting_reorders_the_numbers_if_they_do_not_exactly_match()
        {
            var topics = new TopicToken[]
            {
                new TopicToken{Key = "foo", Title = "foo", Order = 1},
                new TopicToken{Key = "bar", Title = "bar", Order = 3},
                new TopicToken{Key = "aaa", Title="aaa" ,Order = 5},
            };

            theFileSystem.Stub(x => x.ReadTopics()).Return(topics);

            ClassUnderTest.Start();

            theFileSystem.AssertWasCalled(x => x.Reorder(topics[1], 2));
            theFileSystem.AssertWasCalled(x => x.Reorder(topics[2], 3));
        }

        [Test]
        public void if_text_is_blank_stop()
        {
            ClassUnderTest.ReadText("  ").ShouldEqual(WhatNext.Stop);
            ClassUnderTest.ReadText("").ShouldEqual(WhatNext.Stop);
            ClassUnderTest.ReadText(null).ShouldEqual(WhatNext.Stop);
        }

        [Test]
        public void stop_on_q_or_quit()
        {
            ClassUnderTest.ReadText("q").ShouldEqual(WhatNext.Stop);
            ClassUnderTest.ReadText("quit").ShouldEqual(WhatNext.Stop);
        }

        [Test]
        public void adding_a_new_topic_to_a_brand_new_folder()
        {
            theExistingFilesAre();
            ClassUnderTest.Start();

            ClassUnderTest.ReadText("foo=Some foo you are")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 1,
                Title = "Some foo you are"
            }));
        }

        [Test]
        public void adding_a_new_topic_to_an_existing_folder()
        {
            theExistingFilesAre("a", "b", "c");

            ClassUnderTest.Start();

            ClassUnderTest.ReadText("foo=Some foo you are")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 4,
                Title = "Some foo you are"
            }));
        }

        [Test]
        public void adding_a_new_topic_to_an_existing_folder_by_key_only_infers_the_title()
        {
            theExistingFilesAre("a", "b", "c");

            ClassUnderTest.Start();

            ClassUnderTest.ReadText("foo")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 4,
                Title = "Foo"
            }));
        }

        [Test]
        public void adding_a_second_topic_to_an_existing_folder()
        {
            theExistingFilesAre("a", "b", "c");

            ClassUnderTest.Start();

            ClassUnderTest.ReadText("foo=Some foo you are")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 4,
                Title = "Some foo you are"
            }));

            ClassUnderTest.ReadText("bar=moar")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "bar",
                Order = 5,
                Title = "moar"
            }));
        }

        [Test]
        public void adding_a_child_folder_to_a_blank_folder()
        {
            theExistingFilesAre();
            ClassUnderTest.Start();

            ClassUnderTest.ReadText("/foo=Some foo you are")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 1,
                Title = "Some foo you are",
                Type = TopicTokenType.Folder
            }));
        }

        [Test]
        public void adding_a_child_folder_to_an_existing_folder()
        {
            theExistingFilesAre("a","b", "c");
            ClassUnderTest.Start();

            ClassUnderTest.ReadText("/foo=Some foo you are")
                .ShouldEqual(WhatNext.ReadMore);

            theFileSystem.AssertWasCalled(x => x.AddTopic(new TopicToken
            {
                Key = "foo",
                Order = 4,
                Title = "Some foo you are",
                Type = TopicTokenType.Folder
            }));
        }
    }
}