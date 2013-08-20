using System;
using System.Collections.Generic;
using Bottles;
using FubuDocs.Infrastructure;
using FubuMVC.Core;
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
            return FubuApplication.For<RunFubuDocsRegistry>()
                                  .StructureMap(new Container())
                                  .Packages(x => FubuDocsApplication.ConfigureLoaders(x, _directories));
        }
    }
}