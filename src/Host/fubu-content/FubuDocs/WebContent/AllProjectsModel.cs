using System.Collections.Generic;
using System.Linq;
using FubuDocs.Navigation;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs
{
    [UrlPattern("projects")]
    public class AllProjectsModel
    {
        private readonly ICurrentHttpRequest _request;

        public AllProjectsModel(ICurrentHttpRequest request)
        {
            _request = request;
        }

        public HtmlTag Topics
        {
            get
            {
                var projects = TopicGraph.AllTopics.Projects
                                         .OrderBy(x => x.Name);

                return new ProjectTableTag(_request, projects);
            }
        }
    }

    public class ProjectTableTag : TableTag
    {
        public ProjectTableTag(ICurrentHttpRequest request, IEnumerable<ProjectRoot> projects)
        {
            AddClass("table");

            projects.Each(project => {
                AddBodyRow(row => {
                    row.Cell().Append(new TopicLinkTag(request, project.Home, null));
                    row.Cell(project.TagLine).AddClass("project-description");
                });
            });
        }
    }
}