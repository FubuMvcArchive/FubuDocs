using System.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocsRunner.Running
{
    public class RunFubuDocsRegistry : FubuRegistry
    {
        public RunFubuDocsRegistry()
        {
            AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
            Policies.Add<UseDefaultHomeRouteIfNone>();
            Policies.Add<OverrideChrome>();
        }
    }

    [ConfigurationType(ConfigurationType.ModifyRoutes)]
    public class UseDefaultHomeRouteIfNone : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            if (!graph.Behaviors.Where(x => x.Route != null).Any(x => x.GetRoutePattern() == string.Empty))
            {
                graph.BehaviorFor<DefaultHomeEndpoint>(x => x.Index()).Route = new RouteDefinition("");
            }
        }
    }
}