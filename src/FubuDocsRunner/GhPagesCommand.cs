using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using FubuCore.CommandLine;
using FubuCore;
using System.Linq;

namespace FubuDocsRunner
{
    public class GhPagesInput
    {
        [Description("The reference to the git repo where you want the gh-pages branch")]
        public string GitRepository { get; set; }
    }

    [CommandDescription("Seeds the gh-pages branch for a given git repository", Name = "gh-pages")]
    public class GhPagesCommand : FubuCommand<GhPagesInput>
    {
        private string _clonedDirectory;
        private readonly List<IStep> _steps = new List<IStep>();

        private IStep step
        {
            set
            {
                _steps.Add(value);
            }
        }

        public override bool Execute(GhPagesInput input)
        {
            var fileSystem = new FileSystem();

            // git@github.com:DarthFubuMVC/FubuDocs.git
            var name = Path.GetFileNameWithoutExtension(input.GitRepository.Split('/').Last());

            _clonedDirectory = Path.GetTempPath().AppendPath(name);

            if (fileSystem.DirectoryExists(_clonedDirectory))
            {
                Console.WriteLine("Deleting existing directory ");
            }

            buildOutSteps(input, name);

            var success = runSteps();


            Console.WriteLine("Deleting " + _clonedDirectory);
            fileSystem.DeleteDirectory(_clonedDirectory);

            return success;
        }

        private bool runSteps()
        {
            _steps.OfType<GitStep>().Each(x => x.Directory = x.Directory ?? _clonedDirectory);

            foreach (var x in _steps)
            {
                Console.WriteLine(x.Description());
                try
                {
                    if (!x.Execute()) return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }

            return true;
        }

        private void buildOutSteps(GhPagesInput input, string name)
        {
            step = new GitStep
            {
                Directory = Path.GetTempPath(),
                Command = "clone {0} {1}".ToFormat(input.GitRepository, name)
            };

            step = new GitStep
            {
                Command = "checkout --orphan gh-pages"
            };

            step = new GitStep
            {
                Command = "rm -rf ."
            };

            step = new PlaceholderStep(_clonedDirectory);

            step = new GitStep
            {
                Command = "add ."
            };

            step = new GitStep
            {
                Command = "commit -a -m \"initial clean slate\""
            };

            step = new GitStep
            {
                Command = "push origin gh-pages"
            };
        }
    }

    public interface IStep
    {
        string Description();
        bool Execute();
    }

    public class PlaceholderStep : IStep
    {
        private readonly string _directory;
        private string _file;

        public PlaceholderStep(string directory)
        {
            _directory = directory;
            _file = _directory.AppendPath("readme.txt");
        }

        public string Description()
        {
            return "Write file " + _file;
        }

        public bool Execute()
        {
            new FileSystem().WriteStringToFile(_file, "Replace this file w/ real contents!");
            return true;
        }
    }

    public class GitStep : IStep
    {
        public string Command;
        public string Directory;
        public string Description()
        {
            return "'git {0}' in {1}".ToFormat(Command, Directory);
        }

        public bool Execute()
        {
            var git = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "git",
                CreateNoWindow = true,
                WorkingDirectory = Directory,
                Arguments = Command
            };

            var process = System.Diagnostics.Process.Start(git);
            process.WaitForExit();

            return process.ExitCode == 0;
        }


    }
}