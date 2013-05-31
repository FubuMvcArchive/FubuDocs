using System;
using System.Collections.Generic;
using FubuDocs.Infrastructure;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuDocsRunner.Running
{
    public class FubuDocsApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<RunFubuWorldRegistry>()
                                  .StructureMap(new Container())
                                  .Packages(x => {
                                      var directories = AppDomain.CurrentDomain.SetupInformation.AppDomainInitializerArguments;
                                      directories.Each(directory => x.Loader(new DocumentPackageLoader(directory)));
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
            return FubuApplication.For<RunFubuWorldRegistry>()
                                  .StructureMap(new Container())
                                  .Packages(x =>
                                  {
                                      x.Loader(new DocumentPackageLoader(_directory));
                                      x.Loader(new FubuDocsPackageLoader());
                                  });
        }
    }
}