using System;
using FubuDocsRunner.Topics;
using NUnit.Framework;
using FubuCore;

namespace FubuDocs.Tests.Commands
{
    [TestFixture]
    public class ListCommandSmokeTester
    {
        [Test]
        public void write_topics_for_FubuWorld_Docs()
        {
            Environment.CurrentDirectory = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                                              .AppendPath("FubuDocs.Docs");

            new ListCommand().Execute(new ListInput());
        }

        [Test]
        public void write_todos_for_FubuWorld_Docs()
        {
            Environment.CurrentDirectory = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                                              .AppendPath("FubuDocs.Docs");

            new ListCommand().Execute(new ListInput{Mode = ListMode.todo});
        }

        [Test]
        public void write_all_for_FubuWorld_Docs()
        {
            Environment.CurrentDirectory = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                                              .AppendPath("FubuDocs.Docs");

            new ListCommand().Execute(new ListInput { Mode = ListMode.all });
        }

        [Test]
        public void write_topics_under_a_child_folder()
        {
            Environment.CurrentDirectory = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                                              .AppendPath("Sample.Docs", "colors");

            new ListCommand().Execute(new ListInput());
        }

        [Test]
        public void write_topics_under_the_entire_tree()
        {
            Environment.CurrentDirectory = ".".ToFullPath()
                                              .ParentDirectory().ParentDirectory() // project
                                              .ParentDirectory()
                                              .ParentDirectory(); // src

            new ListCommand().Execute(new ListInput());
        }
    }
}