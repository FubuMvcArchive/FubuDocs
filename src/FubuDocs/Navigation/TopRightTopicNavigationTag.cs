using FubuDocs.Topics;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs.Navigation
{
    public class TopRightTopicNavigationTag : HtmlTag
    {
        private readonly ICurrentHttpRequest _request;

        public TopRightTopicNavigationTag(Topic node, ICurrentHttpRequest request, FubuDocsDirectories directories)
            : base("ul")
        {
            _request = request;
            AddClass("nav");
            Style("float", "right");

            Topic previous = node.FindPrevious();
            if (previous != null)
            {
                Add("li/a")
                    .Attr("href", _request.ToRelativeUrl(directories, previous.AbsoluteUrl))
                    .Text("Previous")
                    .Attr("title", previous.Title);
            }

            Topic next = node.FindNext();
            if (next != null)
            {
                Add("li/a")
                    .Attr("href", _request.ToRelativeUrl(directories, next.AbsoluteUrl))
                    .Text("Next")
                    .Attr("title", next.Title);
            }

            //Topic index = node.FindIndex();
            //if (index != null && !ReferenceEquals(node, index))
            //{
            //    Add("li/a")
            //        .Attr("href", index.AbsoluteUrl)
            //        .Text("Index")
            //        .Attr("title", index.Title);
            //}
        }
    }
}