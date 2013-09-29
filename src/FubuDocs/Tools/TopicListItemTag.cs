using HtmlTags;

namespace FubuDocs.Tools
{
    public class TopicListItemTag : HtmlTag
    {
        public TopicListItemTag(TopicToken token) : base("li")
        {
            AddClass("dd-item");
            AddClass("dd3-item");

            Data("id", token.Id);
            Data("title", token.Title);
            Data("url", token.Url);
            Data("key", token.Key);

            Add("div").AddClass("dd-handle").AddClass("dd3-handle");
            var contentTag = Add("div").AddClass("dd3-content");
            contentTag.Add("span").AddClass("topic-title").Text(token.Title);

            if (token.Key != "index")
            {
                contentTag.Add("a").AddClass("close").Data("dismiss", "alert").Text("&times;").Encoded(false);
            }
                

            Append(new TopicNodeTag(token.Children));
        }
    }
}