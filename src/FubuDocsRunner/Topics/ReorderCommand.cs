using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using FubuCore.CommandLine;
using System.Linq;
using FubuCore;

namespace FubuDocsRunner.Topics
{
    public class ReorderInput
    {
        [Description("Directs the command to open a temporary file in which to specify the reordering")]
        public bool FileFlag { get; set; }
    }

    [CommandDescription("Reorder the topics and topic folders within the current directory")]
    public class ReorderCommand : FubuCommand<ReorderInput>
    {
        private readonly ITopicFileSystem _fileSystem = new TopicFileSystem(Environment.CurrentDirectory);

        public override bool Execute(ReorderInput input)
        {
            var topics = _fileSystem.ReadTopics().OrderBy(x => x.Order).ToArray();
            var currentOrder = topics.Select(x => x.Key).Join(", ");

            ConsoleWriter.Write(ConsoleColor.Cyan, "The current order is:");
            ConsoleWriter.Write(ConsoleColor.Cyan, currentOrder);


            if (input.FileFlag)
            {
                reorderByFile(topics);
            }
            else
            {
                reorderInline(topics);
            }

            new ListCommand().Execute(new ListInput());

            return true;
        }

        private void reorderInline(TopicToken[] topics)
        {
            ConsoleWriter.Write(ConsoleColor.Cyan, "Enter the keys in the new order separated by commas");
            var newOrder = Console.ReadLine().ToDelimitedArray();

            reorder(topics, newOrder);
        }

        private void reorder(TopicToken[] topics, string[] keys)
        {
            if (keys.Length != topics.Length)
            {
                throw new CommandFailureException("Wrong number of topic keys");
            }

            var missing = keys.Where(x => !topics.Any(t => t.Key == x));
            if (missing.Any())
            {
                throw new CommandFailureException("Unknown topic key(s) {0}".ToFormat(missing.Join(", ")));
            }

            for (int i = 0; i < keys.Length; i++)
            {
                var topic = topics.Single(x => x.Key == keys[i]);
                var order = i + 1;

                _fileSystem.Reorder(topic, order);
            }
        }

        private void reorderByFile(TopicToken[] topics)
        {
            var file = Path.GetTempFileName() + ".txt";
            ConsoleWriter.Write(ConsoleColor.White, "Generating and opening file " + file);

            new FileSystem().AlterFlatFile(file, list => list.AddRange(topics.Select(x => x.Key)));

            Process.Start(file);
            ConsoleWriter.Write(ConsoleColor.White, "After reordering the keys in the file, press any key to return");
            Console.ReadLine();

            string[] newOrder = null;

            Console.WriteLine("Enter 'accept' to accept or anything else to reject it");
            new FileSystem().AlterFlatFile(file, list => newOrder = list.Where(x => x.IsNotEmpty()).ToArray());
            ConsoleWriter.Write(ConsoleColor.Green,"Got: " + newOrder.Join(", "));

            Console.WriteLine("Type 'accept' to accept the changes or anything else to reject the changes");
            var next = Console.ReadLine();
            try
            {
                if (next == "accept")
                {

                    reorder(topics, newOrder);
                }
            }
            finally
            {
                new FileSystem().DeleteFile(file);
            }

        }

    }
   
}