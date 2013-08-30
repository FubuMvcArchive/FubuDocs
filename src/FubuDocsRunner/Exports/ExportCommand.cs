using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util.TextWriting;
using FubuDocs.Exporting;
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

		[Description("Comma separate list of the projects to include in the export (e.g., fubudocs, myproject)")]
		[FlagAlias("include-projects", 'i')]
		public string IncludeProjectsFlag { get; set; }
    }

    [CommandDescription("Exports static html content for all of the documentation projects in the specified folder")]
    public class ExportCommand : FubuCommand<ExportInput>
    {
        private readonly IFileSystem _fileSystem = new FileSystem();

        public override bool Execute(ExportInput input)
        {
            var runnerDirectory = Assembly.GetExecutingAssembly().Location.ParentDirectory();

            var bottling = Task.Factory.StartNew(() =>
            {
                if (!input.NoBottlingFlag)
                {
                    new BottleCommand().Execute(new BottleInput { NoZipFlag = true });
                }
            });

            var cleaning = Task.Factory.StartNew(() => { cleanExplodedBottleContents(runnerDirectory); });


            Task.WaitAll(bottling, cleaning);


            var directories = input.ToDirectories();


            try
            {
                _fileSystem.CreateDirectory(input.Output);
                _fileSystem.CleanDirectory(input.Output);

				var projects = input
					.IncludeProjectsFlag
					.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim());

	            projects.Each(BottlesFilter.Include);

                var application = new FubuDocsExportingApplication(directories).BuildApplication();
                using (var server = application.RunEmbedded(directories.Solution))
                {
                    var report = startReport();


                    var contentUrls = findInitialContentUrlList(server);
                    Console.WriteLine("Downloading initial batch of {0} urls", contentUrls.Count());
                    contentUrls.Each(x => x.Write(server, input, report));

                    var cache = server.Services.GetInstance<IAccessedCache>();
                    while (cache.Any())
                    {
                        var moreUrls = cache.Dequeue();
                        Console.WriteLine("Downloading an additional {0} discovered url's", moreUrls.Count());

                        moreUrls.Each(x => x.Write(server, input, report));
                    }


                    report.WriteToConsole();
                }  

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static List<ContentUrl> findInitialContentUrlList(EmbeddedFubuMvcServer server)
        {
            var urls = server.Endpoints.Get<UrlQueryEndpoint>(x => x.get_urls())
                             .ReadAsJson<UrlList>();

            var contentUrls = urls.Urls.Select(x => new ContentUrl(x)).ToList();
            contentUrls.Sort();
            return contentUrls;
        }

        private static TextReport startReport()
        {
            var report = new TextReport();
            report.StartColumns(2);
            report.AddColumnData("Url", "File");
            report.AddDivider('-');
            return report;
        }

        private void cleanExplodedBottleContents(string runnerDirectory)
        {
            var explodedBottlesDirectory = runnerDirectory.AppendPath("fubu-content");
            Console.WriteLine("Trying to clean out the contents of " + explodedBottlesDirectory);
            _fileSystem.CleanDirectory(explodedBottlesDirectory);
        }
    }
}