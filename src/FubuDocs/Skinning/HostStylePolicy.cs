using System.Linq;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime.Files;

namespace FubuDocs.Skinning
{
    public class HostStylePolicy : IAssetPolicy
    {
        public const string ThemeCssFile = "fubudocs.theme.css";

        private readonly IFubuApplicationFiles _files;

        public HostStylePolicy(IFubuApplicationFiles files)
        {
            _files = files;
        }

        public void Apply(IPackageLog log, IAssetFileGraph fileGraph, AssetGraph graph)
        {
            var cssFile = _files.FindFiles(FileSet.Shallow(ThemeCssFile)).FirstOrDefault();
            if (cssFile == null) return;

            var file = new AssetFile(ThemeCssFile) {FullPath = cssFile.Path};

            var assetPath = new AssetPath("Application", ThemeCssFile, AssetFolder.scripts);

            fileGraph.AllPackages.FirstOrDefault(x => x.PackageName.EqualsIgnoreCase("application"))
                     .AddFile(assetPath, file);

            graph.AddToCombination("fubudocs", ThemeCssFile);
        }
    }
}