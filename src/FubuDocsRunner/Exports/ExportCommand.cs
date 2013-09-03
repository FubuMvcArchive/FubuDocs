using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
	        OutputFlag = Path.GetTempPath().AppendPath("fubudocs-export");
	        MessageFlag = "FubuDocs export at " + DateTime.UtcNow.ToGmtFormattedDate();
	    }

        [Description("The directory to output the application, default is to the temp directory")]
        public string OutputFlag { get; set; }

		[Description("Comma separate list of the projects to include in the export (e.g., fubudocs, myproject)")]
		[FlagAlias("include-projects", 'i')]
		public string IncludeProjectsFlag { get; set; }

        [Description("If specified, attempts to copy and commit the new content to the gh-pages branch of this repo")]
        public string GitFlag { get; set; }

        [Description("Specify the git commit message if exporting directly to gh-pages")]
        public string MessageFlag { get; set; }

        public string RepoName()
        {
            return GitFlag.IsEmpty() ? null : Path.GetFileNameWithoutExtension(GitFlag.Split('/').Last());
        }
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
                _fileSystem.CreateDirectory(input.OutputFlag);
                _fileSystem.CleanDirectory(input.OutputFlag);

				var projects = input
					.IncludeProjectsFlag
					.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim());

	            projects.Each(BottlesFilter.Include);

                var application = new FubuDocsExportingApplication(directories).BuildApplication();
                using (var server = application.RunEmbedded(directories.Solution))
                {
                    exportContent(input, server);
                }  

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            if (input.GitFlag.IsNotEmpty())
            {
                return publishToGhPages(input);
            }

            return true;
        }



        private static void exportContent(ExportInput input, EmbeddedFubuMvcServer server)
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

        private bool publishToGhPages(ExportInput input)
        {
            Console.WriteLine("Attempting to push the new documentation to " + input.GitFlag);

            var steps = new PublishToGhPages(input);
            try
            {
                return steps.RunSteps();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            finally
            {
                new FileSystem().DeleteDirectory(steps.DefaultDirectory);                
            }
        }
    }

    public class PublishToGhPages : StepCollection
    {
        public PublishToGhPages(ExportInput input) : base(Path.GetTempPath().AppendPath(input.RepoName()))
        {
            Add = new GitStep
            {
                Directory = Path.GetTempPath(),
                Command = "clone {0} {1}".ToFormat(input.GitFlag, input.RepoName())
            };

            Add = new GitStep
            {
                Command = "checkout gh-pages"
            };

            Add = new GitStep
            {
                Command = "rm -rf ."
            };

            Add = new CopyContent(input.OutputFlag, DefaultDirectory);

            Add = new GitStep
            {
                Command = "add ."
            };

            Add = new GitStep
            {
                Command = "commit -a -m \"0\"".ToFormat(input.MessageFlag)
            };

            Add = new GitStep
            {
                Command = "push origin gh-pages"
            };
        }
    }

    public class CopyContent : IStep
    {
        private readonly string _source;
        private readonly string _destination;

        public CopyContent(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public string Description()
        {
            return "Copy all files from {0} to {1}".ToFormat(_source, _destination);
        }

        public bool Execute()
        {
            var fileSystem = new FileSystem();
            var files = fileSystem.FindFiles(_source, FileSet.Everything());
            files.Each(x => {
                var relativePath = x.PathRelativeTo(_source);
                var destinationFile = _destination.AppendPath(relativePath);

                Console.WriteLine("Copying {0} to {1}", x, destinationFile);
            });

            return true;
        }
    }
}