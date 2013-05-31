using System.Collections.Generic;
using System.Linq;
using FubuDocs.Navigation;
using FubuDocs.Topics;
using FubuMVC.Core;
using HtmlTags;

namespace FubuDocs
{
    [UrlPattern("projects")]
    public class AllProjectsModel
    {
        public HtmlTag Topics
        {
            get
            {
                var projects = TopicGraph.AllTopics.Projects
                                         .OrderBy(x => x.Name);

                return new ProjectTableTag(projects);
            }
        }
    }

    public class ProjectTableTag : TableTag
    {
        public ProjectTableTag(IEnumerable<ProjectRoot> projects)
        {
            AddClass("table");

            projects.Each(project => {
                AddBodyRow(row => {
                    row.Cell().Append(new TopicLinkTag(project.Home, null));
                    row.Cell(project.TagLine).AddClass("project-description");
                });
            });
        }
    }
}