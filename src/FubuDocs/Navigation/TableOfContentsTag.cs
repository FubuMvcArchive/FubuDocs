using System.Collections.Generic;
using System.Linq;
using FubuDocs.Topics;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs.Navigation
{
    public class TableOfContentsTag : HtmlTag
    {
        private readonly ICurrentHttpRequest _currentRequest;

        public TableOfContentsTag(Topic topic, ICurrentHttpRequest currentRequest, FubuDocsDirectories directories) : base("ul")
        {
            _currentRequest = currentRequest;
            AddClass("table-of-contents");

            writeChildNodes(topic, this, directories);
        }

        private void writeChildNodes(Topic node, HtmlTag tag, FubuDocsDirectories directories)
        {
            node.ChildNodes.Each(childTopic => {
                var li = tag.Add("li");

                li.Add("a").Attr("href", _currentRequest.ToRelativeUrl(directories, childTopic.AbsoluteUrl)).Text(childTopic.Title);

                if (childTopic.ChildNodes.Any())
                {
                    var ul = li.Add("ul");
                    writeChildNodes(childTopic, ul, directories);
                }
            });
        }
    }
}