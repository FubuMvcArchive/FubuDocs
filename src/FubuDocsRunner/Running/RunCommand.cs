using System;
using System.Reflection;
using System.Threading.Tasks;
using Fubu.Running;
using FubuCore;
using FubuCore.CommandLine;
using HtmlTags;

namespace FubuDocsRunner.Running
{
    [CommandDescription("Runs a FubuDocs documentation project with Katana")]
    public class RunCommand : FubuCommand<RunInput>
    {
        private readonly IFileSystem fileSystem = new FileSystem();
        private RemoteApplication _application;

        public override bool Execute(RunInput input)
        {
            string runnerDirectory = Assembly.GetExecutingAssembly().Location.ParentDirectory();

            Task bottling = Task.Factory.StartNew(() => {
                if (!input.NoBottlingFlag)
                {
                    new BottleCommand().Execute(new BottleInput {NoZipFlag = true});
                }
            });

            Task cleaning = Task.Factory.StartNew(() => { cleanExplodedBottleContents(runnerDirectory); });


            Task.WaitAll(bottling, cleaning);

            var directories = input.ToDirectories();


            try
            {
                _application = new RemoteApplication(x => {
                    
                    x.Setup.AppDomainInitializerArguments = new[] { JsonUtil.ToJson(directories) };

                    x.Setup.ApplicationBase = runnerDirectory;
                });

                _application.Start(input.ToRequest());

                tellUserWhatToDo();
                ConsoleKeyInfo key = Console.ReadKey();
                while (key.Key != ConsoleKey.Q)
                {
                    if (key.Key == ConsoleKey.R)
                    {
                        _application.RecycleAppDomain();
                        new SnippetsCommand().Execute(new SnippetsInput());

                        tellUserWhatToDo();
                    }

                    key = Console.ReadKey();
                }

                _application.Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        private static void tellUserWhatToDo()
        {
            Console.WriteLine("Press 'q' to quit, 'r' to recycle the application");
        }

        private void cleanExplodedBottleContents(string runnerDirectory)
        {
            string explodedBottlesDirectory = runnerDirectory.AppendPath("fubu-content");
            Console.WriteLine("Trying to clean out the contents of " + explodedBottlesDirectory);
            fileSystem.CleanDirectory(explodedBottlesDirectory);
        }
    }

    public class FubuDocsDirectories
    {
        public string Solution;
        public string Host;
    }
}