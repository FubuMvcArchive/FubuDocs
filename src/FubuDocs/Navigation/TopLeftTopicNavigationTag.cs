using FubuDocs.Topics;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuDocs.Navigation
{
    public class TopLeftTopicNavigationTag : HtmlTag
    {
        public TopLeftTopicNavigationTag(Topic node, ICurrentHttpRequest currentRequest) : base("ul")
        {
            AddClass("nav");

            var current = new NamedTopicLinkTag(node, currentRequest);
            current.AddClass("active");

            Append(current);

            Topic parent = node.Parent;
            if (parent != null)
            {
                var tag = new NamedTopicLinkTag(parent, currentRequest);
                Children.Insert(0, tag);
            }

            var index = node.FindIndex();
            if (index != null && !ReferenceEquals(node, index) && !ReferenceEquals(index, parent))
            {
                var indexTag = new NamedTopicLinkTag(node.Project.Index, currentRequest);
                Children.Insert(0, indexTag);
            }
        }
    }
}