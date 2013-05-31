using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuDocs.Tests.Topics
{
    public static class ObjectMother
    {
        public static readonly ProjectRoot ProjectRoot;
        public static readonly Cache<string, Topic> Topics;
        public static readonly IEnumerable<ITopicFile> Files;
        public static BehaviorGraph Behaviors;
        public static TopicGraph TopicGraph;

        static ObjectMother()
        {
            FubuMvcPackageFacility.PhysicalRootPath = ".".ToFullPath().ParentDirectory().ParentDirectory();
            var registry = new FubuRegistry();
            registry.Import<FubuDocsExtension>();

            FubuRuntime app = FubuApplication
                .For(registry)
                .StructureMap(new Container())
                .Bootstrap();

            TopicGraph = TopicGraph.AllTopics;

            Behaviors = app.Factory.Get<BehaviorGraph>();

            ProjectRoot = TopicGraph.AllTopics.ProjectFor("FubuMVC");

            Topics = new Cache<string, Topic>();
            Topics[ProjectRoot.Index.Key] = ProjectRoot.Index;
            ProjectRoot.Index.Descendents().Each(x => Topics[x.Key] = x);

            Files = Topics.Select(x => x.File).ToArray();
        }
    }
}