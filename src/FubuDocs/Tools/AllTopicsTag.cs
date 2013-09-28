using System.Collections.Generic;
using FubuDocs.Topics;
using HtmlTags;

namespace FubuDocs.Tools
{
    public class AllTopicsTag : TableTag
    {
        public AllTopicsTag(string fileUrl, ProjectRoot root)
        {
            AddClass("table");

            AddHeaderRow(tr => {
                tr.Header("Title");
                tr.Header("Key");
                tr.Header("File");
            });

            if (root.Splash != null)
            {
                addTopic(fileUrl, root.Splash);
            }

            root.AllTopics().Each(x => addTopic(fileUrl, x));
        }

        private void addTopic(string fileUrl, Topic topic)
        {
            AddBodyRow(row => {
                row.Cell(topic.Title);
                row.Cell(topic.Key);

                row.Cell().Add("a").Data("url", fileUrl).Data("key", topic.Key).Attr("href", "#").AddClass("edit-link").Text(topic.File.FilePath);
            });
        }
    }
}