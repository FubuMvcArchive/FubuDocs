using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Bottles;
using Bottles.Commands;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;
using System.Linq;
using FubuDocs.Topics;

namespace FubuDocsRunner
{
    public class BottleInput : DocActionInput
    {
        [Description("If selected, disables the creation of the pak-WebContent.zip file")]
        public bool NoZipFlag { get; set; }
    }

    [CommandDescription("Packages up a documentation project as a FubuDocs bottle")]
    public class BottleCommand : FubuCommand<BottleInput>
    {
        private static readonly FileSystem fileSystem = new FileSystem();

        public override bool Execute(BottleInput input)
        {
            importSnippets(input);

            input.DetermineDocumentsFolders().Each(directory => {
                bottleize(input, directory);
            });

            return true;
        }

        private static void bottleize(BottleInput input, string directory)
        {
            writeManifestIfNecessary(directory);
            writeIndexPageIfNecessary(directory);
            writeIndexProjectFileIfNecessary(directory);

            NuspecMaker.CreateNuspecIfMissing(directory);

            gatherNuspecInformation(directory);

            if (!input.NoZipFlag)
            {
                bottleItUp(directory);
            }
        }

        private static void gatherNuspecInformation(string directory)
        {
            string file = directory.AppendPath(ProjectRoot.File);
            var project = ProjectRoot.LoadFrom(file);
            var initial = project.Nugets;

            PublishedNuget.GatherNugetsIntoProject(project, Environment.CurrentDirectory);
            if (initial == null && project.Nugets != null || !Enumerable.SequenceEqual(initial, project.Nugets))
            {
                project.WriteTo(file);
            }

            
        }

        private static void writeIndexProjectFileIfNecessary(string directory)
        {
            var file = directory.AppendPath(ProjectRoot.File);

           if (!fileSystem.FileExists(file))
           {
               var bottleName = Path.GetFileName(directory);
               var guessedName = bottleName.Replace(".Docs", "");
               var project = new ProjectRoot
               {
                   BottleName = bottleName,
                   Name = guessedName,
                   BuildServerUrl =
                       "http://build.fubu-project.org/project.html?projectId=[CHANGEME]&tab=projectOverview",
                   GitHubPage = "http://github.com/DarthFubuMVC/" + guessedName,
                   Url = guessedName.ToLower(),
                   UserGroupUrl = "https://groups.google.com/forum/?fromgroups#!forum/fubumvc-devel"
               };

               Console.WriteLine("Writing documentation project directives to " + file);
               project.WriteTo(file);
           }
        }

        private static void writeIndexPageIfNecessary(string directory)
        {
            var hasIndex = fileSystem.FindFiles(directory, FileSet.Shallow("index.*")).Any();
        
            if (!hasIndex)
            {
                var guessedTitle = Path.GetFileNameWithoutExtension(directory).Replace(".Docs", "");

                var text = @"<!--Title: {0}-->
<ProjectSummary />
<TableOfContents />
".ToFormat(guessedTitle);

                var indexFile = directory.AppendPath("index.spark");
                Console.WriteLine("Writing project root file to " + indexFile);
                
                fileSystem.WriteStringToFile(indexFile, text);
            }
        }

        private static void bottleItUp(string directory)
        {
            new AssemblyPackageCommand().Execute(new AssemblyPackageInput
            {
                RootFolder = directory.ToFullPath()
            });
        }

        private static void writeManifestIfNecessary(string directory)
        {
            string manifestFile = directory.AppendPath(PackageManifest.FILE);
            if (!File.Exists(manifestFile))
            {
                var manifest = new PackageManifest {ContentFileSet = FileSet.Deep("*.*")};
                string assemblyName = Path.GetFileName(directory);
                manifest.Name = assemblyName;
                manifest.AddAssembly(assemblyName);

                manifest.ContentFileSet.Exclude =
                    "Properties/*;bin/*;obj/*;*.csproj*;packages.config;repositories.config;pak-*.zip;*.sln";

                fileSystem.WriteObjectToFile(manifestFile, manifest);
            }
        }

        private static void importSnippets(BottleInput input)
        {
            new SnippetsCommand().Execute(new SnippetsInput {DirectoryFlag = input.DirectoryFlag, QuietFlag = true});
        }
    }

    public static class NuspecMaker
    {
        public static void CreateNuspecIfMissing(string documentationDirectory)
        {
            string name = Path.GetFileName(documentationDirectory);
            string nuspecName = name.ToLower() + ".nuspec";

            string file = ".".ToFullPath().AppendPath("packaging", "nuget", nuspecName);
            var system = new FileSystem();

            if (!system.FileExists(file))
            {
                Console.WriteLine("Creating a nuspec file at " + file);

                string nuspecText = Assembly.GetExecutingAssembly()
                                            .GetManifestResourceStream(typeof (NuspecMaker), "nuspec.txt")
                                            .ReadAllText().Replace("NAME", name);

                Console.WriteLine(nuspecText);

                system.WriteStringToFile(file, nuspecText);
            }
        }
    }
}