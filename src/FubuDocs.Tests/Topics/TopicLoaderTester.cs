using System.Diagnostics;
using FubuDocs.Topics;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class TopicLoaderTester
    {
        [Test]
        public void smoke_test_can_load_project_root_from_folder_outside_of_fubumvc_app()
        {
            var folder =
                ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory().AppendPath("FubuDocs.Docs");

            var project = TopicLoader.LoadFromFolder(folder);

            project.ShouldNotBeNull();
            project.AllTopics().Count().ShouldBeGreaterThan(5);

            project.AllTopics().Each(x => {
                Debug.WriteLine(x);
            });
        }

        [Test]
        public void find_the_project_root_folder_when_you_are_at_the_root()
        {
            var folder =
                ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory().AppendPath("Sample.Docs");

            TopicLoader.FindProjectRootFolder(folder)
                       .ShouldEqual(folder);

        }

        [Test]
        public void find_the_project_root_folder_from_a_child()
        {
            var folder =
                ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory().AppendPath("Sample.Docs");

            TopicLoader.FindProjectRootFolder(folder.AppendPath("colors")).ShouldEqual(folder);
            TopicLoader.FindProjectRootFolder(folder.AppendPath("deep")).ShouldEqual(folder);
            TopicLoader.FindProjectRootFolder(folder.AppendPath("deep", "1.b")).ShouldEqual(folder);
        }

        [Test]
        public void find_the_project_root_from_a_solution_with_multiple_docs_returns_null()
        {
            var folder =
                ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory();

            TopicLoader.FindProjectRootFolder(folder).ShouldBeNull();

        }
    }
}