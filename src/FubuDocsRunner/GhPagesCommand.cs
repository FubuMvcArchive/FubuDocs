using System;
using System.IO;
using FubuCore.CommandLine;
using FubuCore;
using System.Linq;

namespace FubuDocsRunner
{
    [CommandDescription("Seeds the gh-pages branch for a given git repository", Name = "gh-pages")]
    public class GhPagesCommand : FubuCommand<GhPagesInput>
    {
        private string _clonedDirectory;
        private StepCollection _steps;

        public override bool Execute(GhPagesInput input)
        {
            var fileSystem = new FileSystem();

            // git@github.com:DarthFubuMVC/FubuDocs.git
            var name = Path.GetFileNameWithoutExtension(input.GitRepository.Split('/').Last());

            _clonedDirectory = Path.GetTempPath().AppendPath(name);
            _steps = new StepCollection(_clonedDirectory);

            if (fileSystem.DirectoryExists(_clonedDirectory))
            {
                Console.WriteLine("Deleting existing directory ");
            }

            buildOutSteps(input, name);

            var success = _steps.RunSteps();


            Console.WriteLine("Deleting " + _clonedDirectory);
            fileSystem.DeleteDirectory(_clonedDirectory);

            return success;
        }



        private void buildOutSteps(GhPagesInput input, string name)
        {
            _steps.Add = new GitStep
            {
                Directory = Path.GetTempPath(),
                Command = "clone {0} {1}".ToFormat(input.GitRepository, name)
            };

            _steps.Add = new GitStep
            {
                Command = "checkout --orphan gh-pages"
            };

            _steps.Add = new GitStep
            {
                Command = "rm -rf ."
            };

            _steps.Add = new PlaceholderStep(_clonedDirectory);

            _steps.Add = new GitStep
            {
                Command = "add ."
            };

            _steps.Add = new GitStep
            {
                Command = "commit -a -m \"initial clean slate\""
            };

            _steps.Add = new GitStep
            {
                Command = "push origin gh-pages"
            };
        }
    }
}