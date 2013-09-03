using System.Collections.Generic;
using System.IO;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.Packaging;

namespace FubuDocsRunner.Running
{
    public class ApplicationRootPackageLoader : IPackageLoader
    {
        private readonly string _directory;

        public ApplicationRootPackageLoader(string directory)
        {
            _directory = directory;
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            yield return new ContentOnlyPackageInfo(_directory, Path.GetFileName(_directory));
        }
    }
}