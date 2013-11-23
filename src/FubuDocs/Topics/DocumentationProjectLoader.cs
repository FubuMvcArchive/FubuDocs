using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuCore.Logging;
using FubuDocs.Skinning;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuDocs.Topics
{
    [ConfigurationType(ConfigurationType.InjectNodes)]
    public class DocumentationProjectLoader : IConfigurationAction
    {
        private TopicLoader _loader;


        public void Configure(BehaviorGraph graph)
        {
            _loader = new TopicLoader(graph);

            var tasks = PackageRegistry.Packages.Where(x => x.Name.EndsWith(".Docs")).Select(pak => {
                return Task.Factory.StartNew(() => {
                    pak.ForFolder(BottleFiles.WebContentFolder, dir => LoadPackage(pak, dir, graph));
                });
            }).ToArray();

            var directories = graph.Settings.Get<FubuDocsDirectories>();
            if (directories.Host.IsNotEmpty() && !directories.Host.EndsWith(".Docs"))
            {
                var project = loadProject(directories);

                TopicGraph.AllTopics.AddProject(project);
            }

            Task.WaitAll(tasks);

            TopicGraph.AllTopics.ConfigureRelationships();

            if (TopicGraph.AllTopics.Projects.Count() == 1)
            {
                var project = TopicGraph.AllTopics.Projects.Single();
                project.AllTopics().Each(x => { x.Url = x.Url.MoveUp(); });

                project.Home.Url = string.Empty;
            }

            TopicGraph.AllTopics.Projects.Each(project => {
                project.AllTopics().Each(topic => graph.AddChain(topic.BuildChain()));

                if (project.Splash != null)
                {
                    graph.AddChain(project.Splash.BuildChain());
                }
            });
        }

        private ProjectRoot loadProject(FubuDocsDirectories directories)
        {
            var project = _loader.LoadProject("Application", directories.Host);
            project.Name = "Host";
            project.Url = "";
            project.Home.Url = string.Empty;

            project.Index.Descendents().Each(x => x.Url = x.Url.TrimStart('/'));
            return project;
        }

        public void LoadPackage(IPackageInfo pak, string directory, BehaviorGraph graph)
        {
            var root = _loader.LoadProject(pak.Name, directory);


            TopicGraph.AllTopics.AddProject(root);
        }
    }
}