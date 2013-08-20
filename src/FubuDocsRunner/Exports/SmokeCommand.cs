using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.CommandLine;
using FubuDocs.Exporting;
using FubuDocsRunner.Running;
using FubuMVC.Katana;
using System.Linq;
using FubuDocs.Topics;

namespace FubuDocsRunner.Exports
{
    [CommandDescription("Tests each documentation endpoint for Http errors for the doc projects in this folder")]
    public class SmokeCommand : FubuCommand<RunInput>
    {
        private readonly IFileSystem fileSystem = new FileSystem();

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {


            return null;
        }

        public override bool Execute(RunInput input)
        {
            string runnerDirectory = Assembly.GetExecutingAssembly().Location.ParentDirectory();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);


            if (!input.NoBottlingFlag)
            {
                new BottleCommand().Execute(new BottleInput { NoZipFlag = true });
            }
            cleanExplodedBottleContents(runnerDirectory);


            

            var failures = new List<string>();

            try
            {
                var directories = input.ToDirectories();

                // TODO -- It sure would be nice to turn off the pre-compile work so we don't get a ton of console errors
                using (var server = new FubuDocsExportingApplication(directories).BuildApplication().RunEmbedded(directories.Solution))
                {
                    var model = server.Endpoints.Get<UrlQueryEndpoint>(x => x.get_urls()).ReadAsJson<UrlList>();
                    var count = model.Urls.Length;
                    var i = 0;
                    model.Urls.Select(x => server.BaseAddress + x).Each(url => {
                        i++;

                        

                        var status = server.Endpoints.Get(url).StatusCode;
                        string message = "{0}/{1} {2} ({3})".ToFormat(i.ToString().PadLeft(3), count, url, status);

                        if (status != HttpStatusCode.OK)
                        {
                            failures.Add(url);

                            ConsoleWriter.Write(ConsoleColor.Red, message);
                        }
                        else
                        {
                            ConsoleWriter.Write(ConsoleColor.Gray, message);
                        }


                        
                    });

                }

                if (failures.Any())
                {
                    return false;
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
            fileSystem.CleanDirectory(explodedBottlesDirectory);
        }
    }
}