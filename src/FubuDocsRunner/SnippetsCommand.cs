using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util.TextWriting;
using FubuDocsRunner.Running;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Runtime.Files;

namespace FubuDocsRunner
{
    public class SnippetsInput : DocActionInput
    {
        [Description("Scans and lists codesnippets, but does not perform the import")]
        public bool ScanFlag { get; set; }

        [Description("Turns off most of the tracing messages")]
        public bool QuietFlag { get; set; }

        [Description("Finds and lists all the existing code snippets, but does not execute anything")]
        public bool ListFlag { get; set; }
    }

    [CommandDescription(
        "Scrapes the entire solution for files with code snippets and places those files under the '/snippets' directory of the documentation project"
        )]
    public class SnippetsCommand : FubuCommand<SnippetsInput>
    {
        private static readonly FileSystem fileSystem = new FileSystem();

        public override bool Execute(SnippetsInput input)
        {
            var documentsFolders = input.DetermineDocumentsFolders();

            if (input.ListFlag)
            {
                writePreview(documentsFolders);

                return true;
            }


            documentsFolders.Each(dir => processDirectory(input, dir));

            return true;
        }

        private static void writePreview(IEnumerable<string> documentsFolders)
        {
            ConsoleWriter.Write(ConsoleColor.Cyan, "Writing snippet preview, can take a little while to find the snippets....");
            Console.WriteLine();

            var snippets = documentsFolders.Select(buildCache).SelectMany(x => x.All())
                                           .OrderBy(x => x.Name).GroupBy(x => x.Name + "/" + x.File).Select(x => x.First());

            var report = new TextReport();
            report.AddDivider('-');
            report.StartColumns(2);
            report.AddColumnData("Name", "File");
            report.AddDivider('-');

            snippets.Each(x => report.AddColumnData(x.Name, x.File.PathRelativeTo(Environment.CurrentDirectory)));

            report.WriteToConsole();
        }

        private static void processDirectory(SnippetsInput input, string directory)
        {
            ConsoleWriter.Write(ConsoleColor.White, "Processing code snippets for " + directory);

            ISnippetCache cache = buildCache(directory);

            if (input.ScanFlag)
            {
                writePreview(cache);
            }



            string snippets = directory.AppendPath("snippets");

            fileSystem.DeleteDirectory(snippets);

            string srcDirectory = Environment.CurrentDirectory.AppendPath("src");


            Console.WriteLine("Moving snippet files to " + snippets);
            var writer = new TextReport();
            writer.StartColumns(2);
            writer.AddColumnData("Source", "Destination");
            writer.AddDivider('-');

            cache.All().Each(snippet =>
            {
                string relative = snippet.File.PathRelativeTo(srcDirectory).ParentDirectory();
                string newPath = snippets.AppendPath(relative);

                writer.AddColumnData(snippet.File, newPath);

                fileSystem.CopyToDirectory(snippet.File, newPath);
            });

            if (!input.QuietFlag)
            {
                writer.WriteToConsole();
            }
        }

        private static void writePreview(ISnippetCache cache)
        {
            var writer = new TextReport();
            writer.StartColumns(2);

            writer.AddColumnData("Name", "File");
            writer.AddDivider('-');

            cache.All().Each(snippet => { writer.AddColumnData(snippet.Name, snippet.File); });

            writer.WriteToConsole();
        }

        private static ISnippetCache buildCache(string directory)
        {
            var files = new SnippetApplicationFiles(".".ToFullPath().AppendPath("src"), directory);

            var cache = new SnippetCache();

            var scanners = new ISnippetScanner[]
            {
                new CLangSnippetScanner("cs"),
                new CLangSnippetScanner("js"),
                new BlockCommentScanner("<!--", "-->", "spark", "lang-html"),
                new BlockCommentScanner("<!--", "-->", "htm", "lang-html"),
                new BlockCommentScanner("<!--", "-->", "html", "lang-html"),
                new BlockCommentScanner("<!--", "-->", "xml", "lang-xml"),
                new BlockCommentScanner("/*", "*/", "css", "lang-css"),
                new RazorSnippetScanner(),
                new RubySnippetScanner(),
            };

            scanners.Each(finder => {
                files.FindFiles(finder.MatchingFileSet).Each(file => {
                    var scanner = new SnippetReader(file, finder, snippet => {
                        snippet.File = file.Path;
                        cache.Add(snippet);
                    });

                    scanner.Start();
                });
            });


            return cache;
        }
    }

    public class SnippetApplicationFiles : IFubuApplicationFiles
    {
        private readonly List<string> _directories;

        public SnippetApplicationFiles(string sourceDirectory, string documentationDirectory)
        {
            string name = Path.GetFileName(documentationDirectory);

            _directories =
                Directory.GetDirectories(sourceDirectory)
                         .Where(x => !Path.GetFileName(x).EqualsIgnoreCase(name))
                         .ToList();
        }

        public IEnumerable<ContentFolder> Folders { get; private set; }

        public string GetApplicationPath()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            var fileSystem = new FileSystem();

            // I hate the duplication, but it's tooling code on a beautiful Saturday afternoon
            fileSet.AppendExclude(FubuMvcPackageFacility.FubuContentFolder + "/*.*");
            fileSet.AppendExclude(FubuMvcPackageFacility.FubuPackagesFolder + "/*.*");
            fileSet.AppendExclude("snippets/*.*");

            return
                _directories.SelectMany(
                    x => fileSystem.FindFiles(x, fileSet).Select(file => new FubuFile(file, "Unknown")));
        }

        public IFubuFile Find(string relativeName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentFolder> AllFolders { get; private set; }
    }
}