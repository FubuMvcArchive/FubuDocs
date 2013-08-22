using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util.TextWriting;
using FubuDocs.Exporting;
using FubuDocs.Infrastructure;
using FubuDocsRunner.Running;
using FubuMVC.Core.Endpoints;
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
                    var report = startReport();


                    var contentUrls = findInitialContentUrlList(server);
                    Console.WriteLine("Downloading initial batch of {0} urls", contentUrls.Count());
                    contentUrls.Each(x => x.Write(server.Endpoints, input, report));

                    var cache = server.Services.GetInstance<IAccessedCache>();
                    while (cache.Any())
                    {
                        var moreUrls = cache.Dequeue();
                        Console.WriteLine("Downloading an additional {0} discovered url's", moreUrls.Count());

                        moreUrls.Each(x => x.Write(server.Endpoints, input, report));
                    }


                    report.WriteToConsole();
                }  

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
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
            string explodedBottlesDirectory = runnerDirectory.AppendPath("fubu-content");
            Console.WriteLine("Trying to clean out the contents of " + explodedBottlesDirectory);
            _fileSystem.CleanDirectory(explodedBottlesDirectory);
        }
    }



    public class ContentUrl : IComparable<ContentUrl>
    {
        private static readonly IFileSystem fileSystem = new FileSystem();

        private readonly string _relativePath;

        public ContentUrl(string relativePath)
        {
            if (relativePath.StartsWith("../"))
            {
                throw new Exception("Shouldn't be doing this");
            }

            _relativePath = relativePath.TrimStart('/');
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void Write(EndpointDriver endpoints, ExportInput input, TextReport report)
        {
            var localPath = ToLocalPath(input.Output);
            var localDirectory = localPath.ParentDirectory();



            try
            {
                fileSystem.CreateDirectory(localDirectory); // Just making sure
                var content = endpoints.Get(RelativePath).ReadAsText();

                report.AddColumnData(RelativePath, localPath);

                fileSystem.WriteStringToFile(localPath, content);
            }
            catch (Exception e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Failed to write {0} to {1}".ToFormat(RelativePath, localPath));
                Console.WriteLine(e);
            }
        }

        public string ToLocalPath(string root)
        {
            // if _content is in here, do it differently
            if (_relativePath.Contains("_content"))
            {
                return root.AppendPath(_relativePath.Split('/'));
            }

            return root.AppendPath(_relativePath.Split('/')).AppendPath("index.html");
        }

        public int Depth
        {
            get { 
                if (_relativePath.IsEmpty()) return 0;

                return _relativePath.Split('/').Count();
            }
        }

        public int CompareTo(ContentUrl other)
        {
            if (Depth != other.Depth)
            {
                return Depth.CompareTo(other.Depth);
            }

            return _relativePath.CompareTo(other._relativePath);
        }
    }
}