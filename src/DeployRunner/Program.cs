using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FubuCore.CommandLine;
using FubuCore;

namespace DeployRunner
{
    class Program
    {
        public static int Main(string[] args)
        {
            var fileSystem = new FileSystem();

            string srcDirectory = AppDomain.CurrentDomain.BaseDirectory.ParentDirectory().ParentDirectory().ParentDirectory().ParentDirectory();
            string buildDirectory = args.Any()
                                        ? args.Single()
                                        : srcDirectory.ParentDirectory().ParentDirectory().AppendPath("buildsupport");


            Console.WriteLine("Trying to copy to " + buildDirectory);
            if (!fileSystem.DirectoryExists(buildDirectory))
            {
                throw new ApplicationException("Could not find the buildsupport directory");
            }


            var targetDirectory = buildDirectory.AppendPath("FubuDocs");
            
            
            fileSystem.CreateDirectory(targetDirectory);
            fileSystem.CleanDirectory(targetDirectory);

            var fromDirectory = srcDirectory.AppendPath("FubuDocsRunner").AppendPath("bin").AppendPath("debug");
            var files = FileSet.Deep("*.dll;*.pdb;*.exe");
            fileSystem.FindFiles(fromDirectory, files).Each(file => {
                Console.WriteLine("Copying {0} to {1}", file, targetDirectory);
                fileSystem.Copy(file, targetDirectory, CopyBehavior.overwrite);
            });


            return 0;
        }
    }

    
}
