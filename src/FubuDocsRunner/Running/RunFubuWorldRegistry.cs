using FubuMVC.Core;
using FubuMVC.Spark;

namespace FubuDocsRunner.Running
{
    public class RunFubuWorldRegistry : FubuRegistry
    {
        public RunFubuWorldRegistry()
        {
            AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
        }
    }
}