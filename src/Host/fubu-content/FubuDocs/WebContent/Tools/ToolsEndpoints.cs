using System.Collections.Generic;
using System.Linq;
using FubuDocs.Infrastructure;
using FubuDocs.Topics;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuDocs.Tools
{
    public class ToolViewModel
    {
        public string Editor { get; set; }

        public HtmlTag Projects { get; set; }
    }


    public class ToolsEndpoints
    {
        private readonly IUrlRegistry _urls;

        public ToolsEndpoints(IUrlRegistry urls)
        {
            _urls = urls;
        }

        public ToolViewModel get_tools()
        {
            var projectsTag = new HtmlTag("ul");
            TopicGraph.AllTopics.Projects.OrderBy(x => x.Name).Each(x => {
                string url = _urls.UrlFor(new ProjectRequest {Name = x.Name});

                projectsTag.Add("li/a").Text(x.Name).Attr("href", url);
            });

            return new ToolViewModel
            {
                Editor = EditorLauncher.GetEditor(),
                Projects = projectsTag
            };
        }

        public AjaxContinuation post_tools(ToolViewModel model)
        {
            EditorLauncher.SetEditor(model.Editor);

            return AjaxContinuation.Successful();
        }
    }
}