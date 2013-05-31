using System.Collections.Generic;
using FubuCore;
using FubuDocs.Topics;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Files;
using HtmlTags;

namespace FubuDocs.Shared
{
    public class TopNavigation
    {
        public HtmlTag MailingList { get; set; }
        public TagList Social { get; set; }
    }

    public class NavigationEndpoint
    {
        private readonly ITopicContext _context;
        private readonly IAssetUrls _urls;

        public NavigationEndpoint(ITopicContext context, IAssetUrls urls)
        {
            _context = context;
            _urls = urls;
        }

        [FubuPartial]
        public TopNavigation Navigation(TopNavigation navigation)
        {
            var project = _context.Project;
            if (project == null)
            {
                return navigation;
            }

            navigation.Social = new TagList(SocialIconsFor(project));

            if (project.UserGroupUrl.IsNotEmpty())
            {
                navigation.MailingList = new HtmlTag("em")
                    .Append("a", a => a.Attr("href", project.UserGroupUrl).Text(FubuDocsKeys.MailingList));
            }

            return navigation;
        }

        public IEnumerable<HtmlTag> SocialIconsFor(ProjectRoot project)
        {
            if (project.TwitterHandle.IsNotEmpty())
            {
                yield return new HtmlTag("a")
                    .AddClass("ico-twitter")
                    .Attr("href", "http://twitter.com/" + project.TwitterHandle)
                    .Append("img", img =>
                    {
                        img.Attr("alt", "Twitter")
                        .Attr("src", _urls.UrlForAsset(AssetFolder.images, "twitter-icon.png"));
                    });
            }

            if (project.GitHubPage.IsNotEmpty())
            {
                yield return new HtmlTag("a")
                    .AddClass("ico-github")
                    .Attr("href", project.GitHubPage)
                    .Append("img", img =>
                    {
                        img.Attr("alt", "Github")
                        .Attr("src", _urls.UrlForAsset(AssetFolder.images, "github-icon.png"));
                    });
            }
        }
    }
}