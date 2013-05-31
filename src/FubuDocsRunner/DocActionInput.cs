using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuDocs.Topics;
using FubuDocsRunner.Running;

namespace FubuDocsRunner
{
    public class DocActionInput
    {
        [Description(
            "The directory holding the docs.  Will try to find a single directory containing the name 'Docs' under an 'src' folder if this flag is not specified"
            )]
        public string DirectoryFlag { get; set; }

        public IEnumerable<string> DetermineDocumentsFolders()
        {
            if (DirectoryFlag.IsNotEmpty())
            {
                return new[] {DirectoryFlag};
            }

            return TopicLoader.FindDocumentDirectories(Environment.CurrentDirectory);
        }
    }
}