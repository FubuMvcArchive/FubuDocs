using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuDocs.Shared;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark;
using FubuMVC.Spark.SparkModel;

namespace FubuDocsRunner.Running
{
    [ConfigurationType(ConfigurationType.Attributes)]
    public class OverrideChrome : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            TryReplace<TopicChrome>(graph, "TopicChrome.spark");
            TryReplace<SplashChrome>(graph, "SplashChrome.spark");
            TryReplace<TopNavigation>(graph, "NavigationBar.spark");


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

                var descriptor = new ViewDescriptor<Template>(template) { ViewModel = typeof(T) };

                var sparkDescriptor = new SparkDescriptor(descriptor.Template) { ViewModel = typeof(T) };

                var view = new SparkViewToken(sparkDescriptor);

                var chain = graph.Behaviors.Single(x => x.InputType() == typeof(T));
                chain.Output.ClearAll();
                chain.Output.Writers.AddToEnd(new SpecialView<T>(view));
            }
        }
    }

    public class SpecialView<T> : WriterNode
    {
        private readonly ObjectDef _def;

        public SpecialView(IViewToken view)
        {
            _def = new ObjectDef(typeof (SpecialViewWriter<>), typeof (T));

            _def.DependencyByType<IViewFactory>(view.ToViewFactoryObjectDef());
        }

        protected override ObjectDef toWriterDef()
        {
            return _def;
        }

        protected override void createDescription(Description description)
        {
            description.Title = "Special View";
        }

        public override Type ResourceType
        {
            get { return typeof(T); }
        }

        public override IEnumerable<string> Mimetypes
        {
            get { return new string[]{MimeType.Html.ToString()}; }
        }
    }

    public class SpecialViewWriter<T> : IMediaWriter<T>
    {
        private readonly IOutputWriter _writer;
        private readonly IViewFactory _factory;
        private readonly IServiceLocator _services;

        public SpecialViewWriter(IOutputWriter writer, IViewFactory factory, IServiceLocator services)
        {
            _writer = writer;
            _factory = factory;
            _services = services;
        }

        public void Write(string mimeType, T resource)
        {
            var view = _factory.GetView();
            view.Page.ServiceLocator = _services;
            view.Render();

            _writer.ContentType(MimeType.Html);
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.ToString(); }
        }
    }


}