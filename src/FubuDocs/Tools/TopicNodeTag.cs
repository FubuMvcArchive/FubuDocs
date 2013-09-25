using System.Collections.Generic;
using HtmlTags;

namespace FubuDocs.Tools
{
    public class TopicNodeTag : HtmlTag
    {
        public TopicNodeTag(IEnumerable<TopicToken> tokens) : base("ol")
        {
            AddClass("dd-list");

            tokens.Each(x => Append(new TopicListItemTag(x)));
        }
    }
}