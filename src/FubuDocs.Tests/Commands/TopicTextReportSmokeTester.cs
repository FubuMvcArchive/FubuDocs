using FubuDocs.Topics;
using FubuDocsRunner.Topics;
using NUnit.Framework;
using FubuCore;

namespace FubuDocs.Tests.Commands
{
    [TestFixture]
    public class TopicTextReportSmokeTester
    {
        [Test]
        public void write_report_for_FubuWorld_Docs()
        {
            var folder = ".".ToFullPath().ParentDirectory().ParentDirectory().ParentDirectory()
                            .AppendPath("FubuDocs.Docs");

            var root = TopicLoader.LoadFromFolder(folder);

            var report = new TopicTextReport(root.AllTopics());

            report.WriteToConsole();
        }
    }
}