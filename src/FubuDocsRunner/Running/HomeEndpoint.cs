using System.Linq;
using FubuDocs;
using FubuDocs.Skinning;
using FubuDocs.Topics;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;

namespace FubuDocsRunner.Running
{
    public class HomeEndpoint
    {
        private readonly BehaviorGraph _graph;

        public HomeEndpoint(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public FubuContinuation Index()
        {
            var chain = _graph.BehaviorFor<HostHomeEndpoint>(x => x.Render());
            
            if (chain.Output.Writers.OfType<SpecialView<HostHome>>().Any())
            {
                return FubuContinuation.TransferTo<HostHomeEndpoint>(x => x.Render());
            }

            if (TopicGraph.AllTopics.Projects.Count() == 1 &&
                TopicGraph.AllTopics.Projects.Single().BottleName == "FubuDocs.Docs")
            {
                return FubuContinuation.RedirectTo("fubudocs");
            }

            return FubuContinuation.RedirectTo<AllTopicsEndpoint>(x => x.get_topics());
        }
    }
}