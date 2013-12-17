using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Manifest;
using Bottles.PackageLoaders.Assemblies;
using FubuCore;
using FubuDocs.Infrastructure;
using FubuDocs.Topics;
using Spark.Compiler.NodeVisitors;

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
				.SelectMany(dir => loadPackageFromDirectory(dir, reader));
        }

        private static IEnumerable<IPackageInfo> loadPackageFromDirectory(string dir, PackageManifestReader reader)
        {
            Console.WriteLine("Loading documentation from folder " + dir);
            var pak = reader.LoadFromFolder(dir).As<PackageInfo>();
            pak.RemoveAllAssemblies();


            var assemblyFiles = new FileSystem().FindFiles(dir,
                FileSet.Deep("*.Docs.dll", Path.GetFileName(dir) + ".dll"));
            foreach (var file in assemblyFiles)
            {
                var assemblyName = Path.GetFileNameWithoutExtension(file);
                var assembly =
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assemblyName);

                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(file);
                    }
                    catch
                    {
                    }
                }

                if (assembly != null && BottlesFilter.ShouldLoad(assembly))
                {
                    yield return new AssemblyPackageInfo(assembly);
                }
            }

            yield return pak;


        }
    }
}