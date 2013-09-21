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
        private readonly FubuDocsDirectories _directories;

        public AllProjectsModel(ICurrentHttpRequest request, FubuDocsDirectories directories)
        {
            _request = request;
            _directories = directories;
        }

        public HtmlTag Topics
        {
            get
            {
                var projects = TopicGraph.AllTopics.Projects.Where(x => x.Name != "Host")
                                         .OrderBy(x => x.Name);

                return new ProjectTableTag(_request, _directories, projects);
            }
        }
    }

    public class ProjectTableTag : TableTag
    {
        public ProjectTableTag(ICurrentHttpRequest request, FubuDocsDirectories directories, IEnumerable<ProjectRoot> projects)
        {
            AddClass("table");

            projects.Each(project => {
                AddBodyRow(row => {
                    row.Cell().Append(new TopicLinkTag(request, directories, project.Home, null));
                    row.Cell(project.TagLine).AddClass("project-description");
                });
            });
        }
    }
}