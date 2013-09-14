using System;
using System.Collections.Generic;
using System.IO;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuDocs;
using FubuDocs.Topics;
using FubuMVC.Core.Packaging;

namespace FubuDocsRunner.Running
{
    public class CurrentDirectoryLoader : IPackageLoader
    {
        private readonly FubuDocsDirectories _directories;

        public CurrentDirectoryLoader(FubuDocsDirectories directories)
        {
            _directories = directories;
        }

        public IEnumerable<IPackageInfo> Load(IPackageLog log)
        {
            IPackageInfo currentDirectory = null;

            var candidateFile = _directories.Solution.AppendPath("project.xml");
            if (File.Exists(candidateFile))
            {
                try
                {
                    var project = new FileSystem().LoadFromFile<ProjectRoot>(candidateFile);

                    currentDirectory = new ContentOnlyPackageInfo(_directories.Solution, Path.GetFileName(_directories.Solution));
                }
                catch (Exception)
                {
                    // that's right, do nothing here
                    Console.WriteLine("File '{0}' could not be loaded as a project file, so ignoring the current directory", candidateFile);
                }
            }

            if (currentDirectory != null)
            {
                yield return currentDirectory;
            }
        }
    }
}