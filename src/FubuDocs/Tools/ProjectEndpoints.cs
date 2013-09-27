using System.Linq;
using FubuCore.Util;
using FubuDocs.Navigation;
using FubuDocs.Snippets;
using FubuDocs.Todos;
using FubuDocs.Topics;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Urls;
using HtmlTags;
using System.Collections.Generic;

namespace FubuDocs.Tools
{
    public interface ITopicTokenCache
    {
        TopicToken TopicStructureFor(string projectName);
        void RewriteTopicStructure(string projectName, TopicToken newStructure);
    }

    public class TopicTokenCache : ITopicTokenCache
    {
        private readonly Cache<string, TopicToken> _tokens;

        public TopicTokenCache()
        {
            _tokens = new Cache<string, TopicToken>(name => {
                var project = TopicGraph.AllTopics.ProjectFor(name);

                return new TopicToken(project.Index);
            });
        }

        public TopicToken TopicStructureFor(string projectName)
        {
            return _tokens[projectName];
        }

        public void RewriteTopicStructure(string projectName, TopicToken newStructure)
        {
            var original = _tokens[projectName];

            var collector = new DeltaCollector(original, newStructure);
            collector.ExecuteDeltas();

            // Want the new one in on the next try.
            _tokens.Remove(projectName);
        }
    }

    public class ProjectEndpoints
    {
        private readonly IUrlRegistry _urls;
        private readonly ITopicTokenCache _tokenCache;
        private readonly ISnippetCache _cache;

        public ProjectEndpoints(IUrlRegistry urls, ITopicTokenCache tokenCache, ISnippetCache cache)
        {
            _urls = urls;
            _tokenCache = tokenCache;
            _cache = cache;
        }

        public ProjectViewModel get_project_Name(ProjectRequest request)
        {
            var project = TopicGraph.AllTopics.TryFindProject(request.Name);

            var root = _tokenCache.TopicStructureFor(request.Name);

            var fileUrl = _urls.UrlFor<FileRequest>();
            return new ProjectViewModel
            {
                Name = project.Name,
                Project = project,
                Topics = new TopicTreeTag(root),
                Files = new AllTopicsTag(fileUrl, project),
                Snippets = new SnippetsTableTag(_urls, _cache.All()),
                TodoList = new TodoTableTag(fileUrl, TodoTask.FindAllTodos().OrderBy(x => x.File).ThenBy(x => x.Line))
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
        public ProjectViewModel()
        {
            TopicTemplate = new TopicListItemTag(new TopicToken
            {
                Title = string.Empty,
                Url = string.Empty,
                Key = string.Empty
            });
        }

        public string Name { get; set; }
        public ProjectRoot Project { get; set; }

        public HtmlTag TopicTemplate { get; set; }
        public HtmlTag Topics { get; set; }
        public HtmlTag Files { get; set; }
        public HtmlTag Snippets { get; set; }
        public HtmlTag TodoList { get; set; }
    }
}