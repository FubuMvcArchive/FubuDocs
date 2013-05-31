using FubuDocs;
using FubuDocs.Topics;
using FubuMVC.Core.Continuations;
using System.Linq;

namespace FubuDocsRunner.Running
{
    public class HomeEndpoint
    {
        public FubuContinuation Index()
        {
            if (TopicGraph.AllTopics.Projects.Count() == 1 &&
                TopicGraph.AllTopics.Projects.Single().BottleName == "FubuDocs.Docs")
            {
                return FubuContinuation.RedirectTo("fubudocs");
            }

            return FubuContinuation.RedirectTo<AllTopicsEndpoint>(x => x.get_topics());
        }
    }
}