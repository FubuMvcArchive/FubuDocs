using System;
using System.Collections.Generic;
using Bottles;
using FubuDocs.Infrastructure;
using FubuDocsRunner.Exports;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using HtmlTags;
using StructureMap;
using System.Linq;
using FubuCore;

namespace FubuDocsRunner.Running
{
    public class FubuDocsApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<RunFubuDocsRegistry>()
                                  .StructureMap(new Container())
                                  .Packages(x => {
                                      var json = AppDomain.CurrentDomain.SetupInformation.AppDomainInitializerArguments.FirstOrDefault();
                                      var directories = JsonUtil.Get<FubuDocsDirectories>(json);

                                      ConfigureLoaders(x, directories);
                                  });
        }

        public static void ConfigureLoaders(IPackageFacility x, FubuDocsDirectories directories)
        {
            // dirty, dirty hack
            if (directories.Host.IsNotEmpty())
            {
                FubuMvcPackageFacility.PhysicalRootPath = directories.Host.TrimEnd('/').TrimEnd('\\');
            }

            x.Loader(new DocumentPackageLoader(directories.Solution));

            if (directories.Host.IsNotEmpty())
            {
                Console.WriteLine("Loading hosting application at " + directories.Host);
                x.Loader(new ApplicationRootPackageLoader(directories.Host));
            }

            x.Loader(new FubuDocsPackageLoader());
        }
    }

    public class FubuDocsExportingApplication : IApplicationSource
    {
        private readonly FubuDocsDirectories _directories;

        public FubuDocsExportingApplication(FubuDocsDirectories directories)
        {
            _directories = directories;
        }

        public FubuApplication BuildApplication()
        {
            var container = new Container();
            container.Inject(_directories);

            return FubuApplication.For<FubuDocsExportingRegistry>()
                                  .StructureMap(container)
                                  .Packages(x => FubuDocsApplication.ConfigureLoaders(x, _directories));
        }
    }

    public class FubuDocsExportingRegistry : RunFubuDocsRegistry
    {
        public FubuDocsExportingRegistry()
        {
            Services(x => {
                x.ReplaceService<IAssetTagWriter, ExportAssetTagWriter>();
                x.ReplaceService<IAssetUrls, ExportAssetUrls>();
                x.SetServiceIfNone<IAccessedCache, AccessedCache>();
            });
        }
    }
}