using FubuMVC.Core.View;
using HtmlTags;

namespace FubuDocs
{
    public static class InstallationPageExtensions
    {
        public static HtmlTag Nuget(this IFubuPage page, string name)
        {
            return new NugetTag(name);
        }

        public static HtmlTag Gem(this IFubuPage page, string name)
        {
            return new GemTag(name);
        }
    }
}