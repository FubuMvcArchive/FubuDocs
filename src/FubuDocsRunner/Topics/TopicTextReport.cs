using System.Collections.Generic;
using FubuCore.Util.TextWriting;
using FubuDocs.Topics;

namespace FubuDocsRunner.Topics
{
    public class TopicTextReport : TextReport
    {

        public TopicTextReport(IEnumerable<Topic> topics)
        {
            StartColumns(3);
            AddDivider('-');
            AddColumnData("Url", "Title", "Key");
            AddDivider('-');

            topics.Each(topic => AddColumnData(topic.Url, topic.Title, topic.Key));
        }

    }
}