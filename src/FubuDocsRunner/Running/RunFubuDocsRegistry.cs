using FubuDocs;
using FubuDocs.Skinning;
using FubuMVC.Core;
using FubuMVC.Spark;

namespace FubuDocsRunner.Running
{
    public class RunFubuDocsRegistry : FubuRegistry
    {
        public RunFubuDocsRegistry(FubuDocsDirectories directories)
        {
            ReplaceSettings(directories);
            AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
            Policies.Add<OverrideChrome>();
        }
    }
}