using HtmlTags;

namespace FubuDocs
{
    public class GemTag : HtmlTag
    {
        public GemTag(string name) : base("div")
        {
            AddClass("alert");
            AddClass("alert-info");

            Add("i").AddClass("icon-info-sign");

            var url = "http://rubygems.org/gems/" + name.ToLower();

            Add("i").Text(name);
            Add("span").Text(" is distributed as a Ruby Gem at ");
            Add("a").Attr("href", url).Text(url);
            Add("span").Text(" and can be installed from the command line with: ");
            Add("code").Text("gem install " + name.ToLower());
        }
    }
}