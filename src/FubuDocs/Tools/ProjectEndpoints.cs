using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bottles.Services.Messaging;
using Fubu.Running;
using FubuCore;
using FubuDocs.Navigation;
using FubuDocs.Snippets;
using FubuDocs.Todos;
using FubuDocs.Topics;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Urls;
using System.Collections.Generic;

namespace FubuDocs.Tools
{
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
                SubmitUrl = _urls.UrlFor(new TopicToken{ProjectName = request.Name}, "POST"),
                Files = new AllTopicsTag(fileUrl, project),
                Snippets = new SnippetsTableTag(_urls, _cache.All()),
                TodoList = new TodoTableTag(fileUrl, TodoTask.FindAllTodos().OrderBy(x => x.File).ThenBy(x => x.Line))
            };
        }

        public AjaxContinuation post_project_ProjectName(TopicToken topic)
        {
            var existing = _tokenCache.TopicStructureFor(topic.ProjectName);
            var collector = new DeltaCollector(existing, topic);
            var orderedDeltas = collector.OrderedDeltas().ToArray();
            orderedDeltas.Each(x => Debug.WriteLine(x));
            
            collector.ExecuteDeltas();

            Task.Factory.StartNew(() => {
                EventAggregator.SendMessage(new RecycleApplication());
            });

            return AjaxContinuation.Successful();
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
}