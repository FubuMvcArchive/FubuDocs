using System.IO;
using FubuDocsRunner.Topics;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Collections.Generic;

namespace FubuDocs.Tests.Commands.Topics
{
    [TestFixture]
    public class TopicFileSystemTester
    {
        private const string _folder = "topic-files";
        private TopicFileSystem theTopicFiles;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().CleanDirectory(_folder);
            new FileSystem().CreateDirectory(_folder);

            theTopicFiles = new TopicFileSystem(_folder);
        }

        [Test]
        public void write_topic_file_that_is_a_leaf()
        {
            var token = new TopicToken
            {
                Key = "foo",
                Title = "it's my foo",
                Order = 3
            };

            var path = theTopicFiles.WriteFile(token);
            Path.GetFileName(path).ShouldEqual("3.foo.spark");

            Path.GetDirectoryName(path).ShouldEqual(_folder);

            var text = new FileSystem().ReadStringFromFile(path);
            text.ShouldContain("<!--Title: it's my foo-->");
            text.ShouldContain("<markdown>");
            text.ShouldContain("</markdown>");
            text.ShouldContain("TODO(Write content!)");
        }

        [Test]
        public void write_topic_file_that_is_a_new_folder()
        {
            var token = new TopicToken
            {
                Key = "foo",
                Title = "it's my foo",
                Order = 3,
                Type = TopicTokenType.Folder
            };

            var path = theTopicFiles.WriteFile(token);
            path.ShouldEqual(_folder.AppendPath("3.foo", "index.spark"));

            var text = new FileSystem().ReadStringFromFile(path);
            text.ShouldContain("<!--Title: it's my foo-->");
            text.ShouldContain("<markdown>");
            text.ShouldContain("</markdown>");
            text.ShouldContain("TODO(Write content!)");
        }

        [Test]
        public void reading_new_topic_files()
        {
            var topics = new TopicToken[]
            {
                new TopicToken
                {
                    Key = "foo",
                    Title = "it's my foo",
                    Order = 1,
                    Type = TopicTokenType.Folder,
                    RelativePath = "1.foo"
                },

                new TopicToken
                {
                    Key = "bar",
                    Title = "it's my bar",
                    Order = 2,
                    Type = TopicTokenType.File,
                    RelativePath = "2.bar.spark"
                },

                new TopicToken
                {
                    Key = "aaa",
                    Title = "triple a",
                    Order = 3,
                    Type = TopicTokenType.File,
                    RelativePath = "3.aaa.spark"
                }
            };

            topics.Each(x => theTopicFiles.WriteFile(x));

            var readTopics = theTopicFiles.ReadTopics();

            readTopics.ShouldHaveTheSameElementsAs(topics);
        }


        [Test]
        public void reorder_an_existing_topic_file()
        {
            var topics = new TopicToken[]
            {
                new TopicToken
                {
                    Key = "foo",
                    Title = "it's my foo",
                    Order = 1,
                    Type = TopicTokenType.Folder,
                    RelativePath = "1.foo"
                },

                new TopicToken
                {
                    Key = "bar",
                    Title = "it's my bar",
                    Order = 2,
                    Type = TopicTokenType.File,
                    RelativePath = "2.bar.spark"
                },

                new TopicToken
                {
                    Key = "aaa",
                    Title = "triple a",
                    Order = 3,
                    Type = TopicTokenType.File,
                    RelativePath = "3.aaa.spark"
                }
            };

            topics.Each(x => theTopicFiles.WriteFile(x));

            var theTopicGettingMoved = topics[2];
            theTopicFiles.Reorder(theTopicGettingMoved, 2);

            File.Exists(_folder.AppendPath("2.aaa.spark"))
                .ShouldBeTrue();

            File.Exists(_folder.AppendPath("3.aaa.spark"))
                .ShouldBeFalse();

            theTopicGettingMoved.Order.ShouldEqual(2);





        }

    }
}