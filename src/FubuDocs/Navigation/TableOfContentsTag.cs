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

        public TableOfContentsTag(Topic topic, ICurrentHttpRequest currentRequest) : base("ul")
        {
            _currentRequest = currentRequest;
            AddClass("table-of-contents");

            writeChildNodes(topic, this);
        }

        private void writeChildNodes(Topic node, HtmlTag tag)
        {
            node.ChildNodes.Each(childTopic => {
                var li = tag.Add("li");

                li.Add("a").Attr("href", _currentRequest.ToRelativeUrl(childTopic.AbsoluteUrl)).Text(childTopic.Title);

                if (childTopic.ChildNodes.Any())
                {
                    var ul = li.Add("ul");
                    writeChildNodes(childTopic, ul);
                }
            });
        }
    }
}