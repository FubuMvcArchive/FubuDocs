using System.Collections.Generic;
using System.Linq;
using Bottles;
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
            PackageRegistry.Packages.Where(x => x.Name.EndsWith(".Docs"))
                .Each(pak => {
                    pak.ForFolder(BottleFiles.WebContentFolder, dir => LoadPackage(pak, dir, graph));
                });

            TopicGraph.AllTopics.ConfigureRelationships();

            if (TopicGraph.AllTopics.Projects.Count() == 1)
            {
                var project = TopicGraph.AllTopics.Projects.Single();
                project.AllTopics().Each(x => {
                    x.Url = x.Url.MoveUp();
                });

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

        public void LoadPackage(IPackageInfo pak, string directory, BehaviorGraph graph)
        {
            ProjectRoot root = _loader.LoadProject(pak.Name, directory);


            TopicGraph.AllTopics.AddProject(root);
        }
    }
}