using System;
using System.Collections.Generic;
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
                                      var directories =
                                          JsonUtil.Get<FubuDocsDirectories>(
                                              AppDomain.CurrentDomain.SetupInformation.AppDomainInitializerArguments
                                                       .FirstOrDefault());

                                      x.Loader(new DocumentPackageLoader(directories.Solution));

                                      if (directories.ApplicationRoot.IsNotEmpty())
                                      {
                                          x.Loader(new ApplicationRootPackageLoader(directories.ApplicationRoot));
                                      }

                                      x.Loader(new FubuDocsPackageLoader());
                                  });
        }
    }

    public class FubuDocsExportingApplication : IApplicationSource
    {
        private readonly string _directory;

        public FubuDocsExportingApplication(string directory)
        {
            _directory = directory;
        }

        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<RunFubuDocsRegistry>()
                                  .StructureMap(new Container())
                                  .Packages(x =>
                                  {
                                      x.Loader(new DocumentPackageLoader(_directory));
                                      x.Loader(new FubuDocsPackageLoader());
                                  });
        }
    }
}