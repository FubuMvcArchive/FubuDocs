using FubuDocs.Skinning;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Routes;

namespace FubuDocs.Topics
{
    [ConfigurationType(ConfigurationType.InjectNodes)]
    public class SetHomePage : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            bool removeHomeEndpoint = false;
            var homeChain = graph.BehaviorFor<HomeEndpoint>(x => x.Index());

            if (TopicGraph.AllTopics.Projects.Count() == 1)
            {
                var project = TopicGraph.AllTopics.Projects.Single();
                var chain = graph.Behaviors.FirstOrDefault(x => x.Route != null && x.GetRoutePattern().EqualsIgnoreCase(project.Url));
                if (chain != null)
                {
                    chain.AddRouteAlias(new RouteDefinition(""));
                    removeHomeEndpoint = true;
                }
            }
            else
            {
                var chain = graph.BehaviorFor<HostHomeEndpoint>(x => x.Render());
                if (chain.Output.Writers.Any(x => x is SpecialView<HostHome>))
                {
                    chain.AddRouteAlias(new RouteDefinition(""));
                    removeHomeEndpoint = true;
                }
            }

            if (removeHomeEndpoint)
            {
                graph.RemoveChain(homeChain);
            }
        }
    }
}