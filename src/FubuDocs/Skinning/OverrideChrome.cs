using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuDocs.Shared;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark;
using FubuMVC.Spark.SparkModel;

namespace FubuDocs.Skinning
{
    [ConfigurationType(ConfigurationType.Attributes)]
    public class OverrideChrome : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            TryReplace<TopicChrome>(graph, "TopicChrome.spark");
            TryReplace<SplashChrome>(graph, "SplashChrome.spark");
            TryReplace<TopNavigation>(graph, "NavigationBar.spark");
            TryReplace<HostHome>(graph, "Index.spark");
        }

        public void TryReplace<T>(BehaviorGraph graph, string fileName)
        {
            var file = graph.Files.FindFiles(FileSet.Shallow(fileName))
                            .FirstOrDefault(x => x.Provenance.EqualsIgnoreCase("application"));

            if (file != null)
            {
                var template = new Template
                {
                    FilePath = file.Path,
                    RootPath = file.Path,
                    ViewPath = Path.GetFileName(file.Path)
                };

                var descriptor = new ViewDescriptor<Template>(template) {ViewModel = typeof (T)};

                var sparkDescriptor = new SparkDescriptor(descriptor.Template) {ViewModel = typeof (T)};

                var view = new SparkViewToken(sparkDescriptor);

                var chain = graph.Behaviors.Single(x => x.ResourceType() == typeof (T));
                chain.Output.ClearAll();
                chain.Output.Writers.AddToEnd(new SpecialView<T>(view));
            }
        }
    }

}