using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using FubuCore;
using FubuDocs.Tools;
using FubuDocs.Topics;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuDocs.Tests.Tools
{
    [TestFixture]
    public class TopicTokenLoadingTester
    {
        private TopicToken root;
        private string directory;

        [SetUp]
        public void SetUp()
        {
            directory = ".".ToFullPath().ParentDirectory().ParentDirectory()
                .ParentDirectory()
                .AppendPath("Sample.Docs");

            root = TopicToken.LoadIndex(directory);

            Debug.WriteLine(JsonUtil.ToJson(root));
        }

        [Test]
        public void reads_file_from_top()
        {
            root.File.ShouldEqual(directory.AppendPath("index.spark"));
        }

        [Test]
        public void is_index_of_root()
        {
            root.IsIndex.ShouldBeTrue();
        }

        [Test]
        public void reads_the_title_of_the_root()
        {
            root.Title.ShouldEqual("Sample Docs");
        }

        [Test]
        public void url_of_the_root()
        {
            root.Url.ShouldEqual("fubumvc");
        }

        [Test]
        public void key_of_the_root()
        {
            root.Key.ShouldEqual("index");
        }



        [Test]
        public void find_some_comments()
        {
            TopicBuilder.FindComments(@"
a
b
<!--Title: foo-->
c
d
<!--Url: bar-->
e
f
").Each(x => Debug.WriteLine(x));



        }


    }
}