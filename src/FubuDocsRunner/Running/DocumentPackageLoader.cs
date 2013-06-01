using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Manifest;
using FubuCore;
using FubuDocs.Infrastructure;
using FubuDocs.Topics;

namespace FubuDocsRunner.Running
{
    public class DocumentPackageLoader : IPackageLoader
    {
        private readonly string _directory;

        public DocumentPackageLoader(string directory)
        {
            _directory = directory;
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            var reader = new PackageManifestReader(new FileSystem(), folder => folder);
            var docDirs = TopicLoader.FindDocumentDirectories(_directory);

            return docDirs
				.Where(BottlesFilter.ShouldLoad)
				.Select(reader.LoadFromFolder);
        }
    }
}