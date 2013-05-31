using FubuDocs.Navigation;
using FubuDocs.Topics;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Collections.Generic;

namespace FubuDocs.Tools
{
    public class ProjectEndpoints
    {
        private readonly IUrlRegistry _urls;

        public ProjectEndpoints(IUrlRegistry urls)
        {
            _urls = urls;
        }

        public ProjectViewModel get_project_Name(ProjectRequest request)
        {
            var project = TopicGraph.AllTopics.TryFindProject(request.Name);

            
        
            return new ProjectViewModel
            {
                Name = project.Name,
                Project = project,
                Topics = new AllTopicsTag(_urls.UrlFor<FileRequest>(), project)
            };
        }

        public AjaxContinuation post_project_Name(ProjectViewModel model)
        {
            var project = TopicGraph.AllTopics.TryFindProject(model.Name);

            model.Project.WriteTo(project.Filename);
            model.Project.Index = project.Index;
            model.Project.Splash = project.Splash;

            TopicGraph.AllTopics.AddProject(model.Project);

            return AjaxContinuation.Successful();
        }
    }

    public class AllTopicsTag : TableTag
    {
        public AllTopicsTag(string fileUrl, ProjectRoot root)
        {
            AddClass("table");

            AddHeaderRow(tr => {
                tr.Header("Title");
                tr.Header("Key");
                tr.Header("File");
            });

            if (root.Splash != null)
            {
                addTopic(fileUrl, root.Splash);
            }

            root.AllTopics().Each(x => addTopic(fileUrl, x));
        }

        private void addTopic(string fileUrl, Topic topic)
        {
            AddBodyRow(row => {
                row.Cell(topic.Title);
                row.Cell(topic.Key);

                row.Cell().Add("a").Data("url", fileUrl).Data("key", topic.Key).Attr("href", "#").AddClass("edit-link").Text(topic.File.FilePath);
            });
        }
    }

    public class ProjectRequest
    {
        public string Name { get; set; }
    }

    public class ProjectViewModel
    {
        public string Name { get; set; }
        public ProjectRoot Project { get; set; }

        public AllTopicsTag Topics { get; set; }
    }
}