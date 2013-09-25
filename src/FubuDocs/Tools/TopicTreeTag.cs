using HtmlTags;

namespace FubuDocs.Tools
{
    public class TopicTreeTag : HtmlTag
    {
        public TopicTreeTag(TopicToken token) : base("div")
        {
            AddClass("dd");
            Id("topic-tree");

            Append(new TopicNodeTag(new [] {token}));
        }
    }
}