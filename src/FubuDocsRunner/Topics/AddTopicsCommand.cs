using System;
using FubuCore.CommandLine;

namespace FubuDocsRunner.Topics
{
    [CommandDescription("Add one or more topics to a directory fast", Name = "add-topics")]
    public class AddTopicsCommand : FubuCommand<AddTopicsInput>
    {
        public override bool Execute(AddTopicsInput topicsInput)
        {
            ConsoleWriter.Write(ConsoleColor.Cyan, "Add a new topic file by using the pattern");
            ConsoleWriter.Write(ConsoleColor.Cyan, "'key=topic' for a new file");
            ConsoleWriter.Write(ConsoleColor.Cyan, "'/key=topic' for a new sub folder");
            ConsoleWriter.Write(ConsoleColor.Cyan, "'enter a blank line, 'q', or 'quit' to finish");

            var controller = new BatchAdderController(new TopicFileSystem(Environment.CurrentDirectory));

            var text = Console.ReadLine();
            while (controller.ReadText(text) == WhatNext.ReadMore)
            {
                text = Console.ReadLine();
            }

            Console.WriteLine();
            new ListCommand().Execute(new ListInput());

            return true;
        }
    }
}