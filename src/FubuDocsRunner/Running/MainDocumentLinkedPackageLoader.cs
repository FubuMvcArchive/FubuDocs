using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Manifest;
using FubuCore;

namespace FubuDocsRunner.Running
{
    public class MainDocumentLinkedPackageLoader : IPackageLoader
    {
        private readonly string _directory;

        public MainDocumentLinkedPackageLoader(string directory)
        {
            _directory = directory;
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var reader = new PackageManifestReader(new FileSystem(), folder => folder);
            yield return reader.LoadFromFolder(_directory);
        }
    }
}