using FubuCore;
using FubuDocs.Topics;
using FubuDocsRunner.Topics;
using NUnit.Framework;

namespace FubuDocs.Tests.Commands
{
    [TestFixture]
    public class TodoTextReportSmokeTester
    {
        [Test]
        public void write_report_for_FubuWorld_Docs()
        {
            var folder = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                .AppendPath("FubuDocs.Docs");

            var root = TopicLoader.LoadFromFolder(folder);

            var report = new TodoTextReport(folder, root.AllTopics());

            report.WriteToConsole();
        }
    }
}