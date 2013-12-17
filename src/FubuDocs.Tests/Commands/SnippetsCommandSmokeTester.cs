using System;
using FubuDocsRunner;
using NUnit.Framework;
using FubuCore;

namespace FubuDocs.Tests.Commands
{
    [TestFixture]
    public class SnippetsCommandSmokeTester
    {
        [Test]
        public void smoke_test_the_preview_mode()
        {
            var directory = ".".ToFullPath()
                               .ParentDirectory().ParentDirectory() // project
                               .ParentDirectory() // src
                               .ParentDirectory(); // tree

            Environment.CurrentDirectory = directory;


            new SnippetsCommand().Execute(new SnippetsInput {ListFlag = true});
        }
    }
}