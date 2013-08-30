using System.Collections.Generic;
using FubuDocs.Topics;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs.Navigation
{
    public class TopicTreeBuilder
    {
        private readonly ICurrentHttpRequest _request;
        private readonly Topic _topic;

        public TopicTreeBuilder(ITopicContext context, ICurrentHttpRequest request)
        {
            _request = request;
            _topic = context.Current;
        }

        public HtmlTag Title()
        {
            return
                new HtmlTag("h1").AddClass("no-margin")
                                 .Text(_topic.Title)
                                 .After(new HtmlTag("hr").AddClass("header-line"));
        }

        public IEnumerable<HtmlTag> BuildTopTopicLinks()
        {
            yield return new TopLeftTopicNavigationTag(_topic, _request);
            yield return new TopRightTopicNavigationTag(_topic, _request);
        }


        public IEnumerable<HtmlTag> BuildLeftTopicLinks()
        {
            var next = _topic.FindNext();

            if (next != null)
            {
                yield return new HtmlTag("h3").AddClass("no-margin").Text("Next");
                yield return new HtmlTag("p", tag => tag.Append(new TopicLinkTag(_request, next, null)));
            }

            var previous = _topic.FindPrevious();

            if (previous != null)
            {
                yield return new HtmlTag("h3").AddClass("no-margin").Text("Previous");
                yield return new HtmlTag("p", tag => tag.Append(new TopicLinkTag(_request, previous, null)));
            }
        }

        public HtmlTag BuildTableOfContents()
        {
            if (_topic == null) return new HtmlTag("div").Render(false);

            return new HtmlTag("div")
                .Append("h2", h2 => h2.AddClass("half-margin").Text("Table of Contents"))
                .Append(new TableOfContentsTag(_topic, _request));
        }
    }
}