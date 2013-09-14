using FubuCore;
using FubuDocs.Topics;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs.Navigation
{
    public class TopicLinkTag : HtmlTag
    {
        public TopicLinkTag(ICurrentHttpRequest currentRequest, FubuDocsDirectories directories, Topic topic, string title) : base("a")
        {
            Attr("href", currentRequest.ToRelativeUrl(directories, topic.AbsoluteUrl));

            if (title.IsEmpty()) title = topic.Title;
            Text(title);
            Attr("data-key", topic.Name);
        }
    }
}