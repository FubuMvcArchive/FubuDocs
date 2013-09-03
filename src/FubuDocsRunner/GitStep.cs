using System.Diagnostics;
using FubuCore;

namespace FubuDocsRunner
{
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