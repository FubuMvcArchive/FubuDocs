using FubuCore;
using HtmlTags;

namespace FubuDocs
{
    public class NugetTag : HtmlTag
    {
        public NugetTag(string name) : base("div")
        {
            AddClass("alert");
            AddClass("alert-info");

            Add("i").AddClass("icon-info-sign");

            var url = "http://www.nuget.org/packages/" + name;

            Add("i").Text(name);
            Add("span").Text(" is distributed as a Nuget at ");
            Add("a").Attr("href", url).Text(url);
            Add("span").Text(" and can be installed from the command line with either:");

            Add("p/code").Text("ripple install " + name + " -p {your project name}");
            Add("p").Text(" - or -");
            Add("p/code").Text("PM> Install-Package " + name);
        }
    }
}