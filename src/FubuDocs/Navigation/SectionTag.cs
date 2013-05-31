using HtmlTags;

namespace FubuDocs.Navigation
{
    public class SectionTag : HtmlTag
    {
        public SectionTag(string text, string id) : base("section")
        {
            Add("h4").Text(text).AddClass("section-header").Id(id);
        }
    }
}