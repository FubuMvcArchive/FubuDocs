using FubuDocs.Skinning;
using FubuMVC.Core;
using FubuMVC.Spark;

namespace FubuDocsRunner.Running
{
    public class RunFubuDocsRegistry : FubuRegistry
    {
        public RunFubuDocsRegistry()
        {
            AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
            Policies.Add<OverrideChrome>();
        }
    }
}