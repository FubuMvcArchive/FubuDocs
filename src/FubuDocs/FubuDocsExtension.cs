using System;
using FubuCore.Binding;
using FubuDocs.CLI;
using FubuDocs.Infrastructure.Binders;
using FubuDocs.Navigation;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.View;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocs
{
    public class FubuDocsExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.AlterSettings<CommonViewNamespaces>(x => {
                x.AddForType<FubuDocsRegistry>();
                x.AddForType<TopicLinkTag>();
                x.AddForType<CommandSectionTag>();
                x.Add("FubuMVC.CodeSnippets");
            });

            registry.Services(x => {
                x.AddService<IPropertyBinder, RequestLogPropertyBinder>();
                x.ReplaceService<ITopicContext, TopicContext>();
                x.ReplaceService<ICommandDocumentationSource, CommandDocumentationSource>();
            });

            registry.ReplaceSettings(TopicGraph.AllTopics);
            registry.Policies.Add<DocumentationProjectLoader>();
        }
    }
}