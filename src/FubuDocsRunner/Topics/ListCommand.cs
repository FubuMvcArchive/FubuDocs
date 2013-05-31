using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.CommandLine;
using FubuDocs.Topics;
using System.Linq;

namespace FubuDocsRunner.Topics
{
    [CommandDescription("Lists the topics and/or todo's in a document project directory")]
    public class ListCommand : FubuCommand<ListInput>
    {
        public ListCommand()
        {
            Usage("Lists all the topics under this directory");
            Usage("Lists the selected reports for the topics under this directory").Arguments(x => x.Mode);
        }

        public override bool Execute(ListInput input)
        {
            var folder = Environment.CurrentDirectory;
            var projectFolder = TopicLoader.FindProjectRootFolder(folder);
            if (projectFolder == null)
            {
                var docFolders = TopicLoader.FindDocumentDirectories(folder);
                if (docFolders.Any())
                {
                    showAllFolders(input, docFolders);
                    return true;
                }
                else
                {
                    ConsoleWriter.Write(ConsoleColor.Yellow, "No documentation projects found");
                    return false;
                }
            }

            WriteProject(input, projectFolder, x => x.File.FilePath.StartsWith(folder));


            return true;
        }

        private void showAllFolders(ListInput input, IEnumerable<string> docFolders)
        {
            docFolders.Each(dir => {
                WriteProject(input, dir, x => true);
                Console.WriteLine();
                Console.WriteLine();
            });
        }

        public static void WriteProject(ListInput input, string projectFolder, Func<Topic, bool> filter)
        {
            var project = TopicLoader.LoadFromFolder(projectFolder);
            var topics = project.AllTopics().Where(filter).ToArray();


            ConsoleWriter.Write(ConsoleColor.Cyan, "Report for " + projectFolder);
            Console.WriteLine();
            if (input.Mode == ListMode.topics || input.Mode == ListMode.all)
            {
                new TopicTextReport(topics).WriteToConsole();
            }

            if (input.Mode == ListMode.all)
            {
                Console.WriteLine();
                Console.WriteLine();
            }

            if (input.Mode == ListMode.all || input.Mode == ListMode.todo)
            {
                new TodoTextReport(projectFolder, topics).WriteToConsole();
            }
        }
    }

    public enum ListMode
    {
        topics,
        all,
        todo
    }

    public class ListInput
    {
        public ListInput()
        {
            Mode = ListMode.topics;
        }

        [Description("Choose what gets listed for the current document directory.  Default is 'topics'")]
        public ListMode Mode { get; set; }
    }
}