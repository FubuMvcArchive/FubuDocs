using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuDocs.Infrastructure;
using FubuDocsRunner.Running;
using FubuMVC.Katana;

namespace FubuDocsRunner.Exports
{
    public class ExportInput : RunInput
    {
	    public ExportInput()
	    {
		    IncludeProjectsFlag = "";
	    }

        [Description("The directory to output the application")]
        public string Output { get; set; }
        
        [Description("Override the root path forcing all urls to be relative (e.g., 'make everything relative to /ripple')")]
        [FlagAlias("root", 'r')]
        public string RootPathFlag { get; set; }

        [Description("Output report of all downloaded files")]
        public bool VerboseFlag { get; set; }

		[Description("Comma separate list of the projects to include in the export (e.g., fubudocs, myproject)")]
		[FlagAlias("include-projects", 'i')]
		public string IncludeProjectsFlag { get; set; }


        public IEnumerable<IDownloadReportVisitor> Visitors()
        {
            if (VerboseFlag)
            {
                yield return new DisplayDownloadReport();
            }

            if (RootPathFlag.IsNotEmpty())
            {
                yield return new OverrideRootPath(RootPathFlag);
            }
        }
    }

    [CommandDescription("Exports static html content for all of the documentation projects in the specified folder")]
    public class ExportCommand : FubuCommand<ExportInput>
    {
        private readonly IFileSystem _fileSystem = new FileSystem();

        public override bool Execute(ExportInput input)
        {
            string runnerDirectory = Assembly.GetExecutingAssembly().Location.ParentDirectory();

            Task bottling = Task.Factory.StartNew(() =>
            {
                if (!input.NoBottlingFlag)
                {
                    new BottleCommand().Execute(new BottleInput { NoZipFlag = true });
                }
            });

            Task cleaning = Task.Factory.StartNew(() => { cleanExplodedBottleContents(runnerDirectory); });


            Task.WaitAll(bottling, cleaning);


            var directories = input.ToDirectories();


            try
            {
                _fileSystem.DeleteDirectory(input.Output);

				var projects = input
					.IncludeProjectsFlag
					.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim());

	            projects.Each(BottlesFilter.Include);

                var application = new FubuDocsExportingApplication(directories).BuildApplication();
                using (var server = application.RunEmbedded(directories.Solution))
                {
                    server.Export(export =>
                    {
                        export.OutputTo(input.Output);
                        input.Visitors().Each(export.AddVisitor);
                    });
                }  

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private void cleanExplodedBottleContents(string runnerDirectory)
        {
            string explodedBottlesDirectory = runnerDirectory.AppendPath("fubu-content");
            Console.WriteLine("Trying to clean out the contents of " + explodedBottlesDirectory);
            _fileSystem.CleanDirectory(explodedBottlesDirectory);
        }
    }
}