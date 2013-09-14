using System.Diagnostics;
using FubuDocs.Navigation;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocs.Tests.Topics
{
    [TestFixture]
    public class ProjectRootImportIntegratedTester
    {
        [Test]
        public void the_imported_docs_project_is_removed_from_the_topic_graph()
        {
            ObjectMother.TopicGraph.Projects.Any(x => x.Name == "Imported")
                .ShouldBeFalse();
        }

        [Test]
        public void importdocs_is_attached_into_parent_project_in_the_right_place()
        {
            var parent = ObjectMother.Topics["fubumvc/nested"];
            parent.FirstChild.NextSibling.Key.ShouldEqual("imported");
        
            parent.ChildNodes.Select(x => x.Key)
                .ShouldHaveTheSameElementsAs("fubumvc/nested/a", "imported", "fubumvc/nested/c");
        }

        [Test]
        public void write_the_table_of_contents_tag_includes_the_newly_imported_project()
        {
            var topic = ObjectMother.TopicGraph.ProjectFor("FubuMVC").Index;
            var html = new TableOfContentsTag(topic, new StandInCurrentHttpRequest
            {
                TheRelativeUrl = "something",
                TheRawUrl = "/something"
            }, new FubuDocsDirectories()).ToString();

            Debug.WriteLine(html);

            html.ShouldContain("The Imported Project");
        }

        [Test]
        public void the_imported_project_is_removed()
        {
            ObjectMother.TopicGraph.Projects.Each(x => Debug.WriteLine(x.BottleName));
            ObjectMother.TopicGraph.Projects.Any(x => x.BottleName == "Imported.Docs")
                .ShouldBeFalse();
        }

        [Test]
        public void all_topics_for_the_root_project_point_at_the_new_root_after_the_import()
        {
            var project = ObjectMother.TopicGraph.ProjectFor("fubumvc");
            project.AllTopics().Each(x => x.Project.ShouldBeTheSameAs(project));
        }

        [Test]
        public void imported_topic_urls_are_inlined()
        {
            var project = ObjectMother.TopicGraph.ProjectFor("fubumvc");

            project.AllTopics().Each(x => Debug.WriteLine(x.Key));

            var topic = project.FindByKey("imported");

            topic.ShouldNotBeNull();

            topic.Url.ShouldEqual("fubumvc/nested/b");

            topic.Descendents().Each(x => {
                x.Url.ShouldStartWith("fubumvc/nested/b");
            });
        }

    }
}