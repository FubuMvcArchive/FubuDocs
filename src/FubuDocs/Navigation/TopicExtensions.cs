using System;
using System.Web;
using FubuCore;
using FubuDocs.Topics;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using HtmlTags;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocs.Navigation
{
    public static class TopicExtensions
    {
        public static HtmlTag BottleSnippetFor(this IFubuPage page, string snippetName)
        {
            var project = page.Get<ITopicContext>().Project;
            var snippets = page.Get<ISnippetCache>();

            if (project == null)
            {
                return page.CodeSnippet(snippetName);
            }

            // TODO -- get rid of the downcast here when the new SlickGrid bottle is ready
            var snippet = snippets.As<SnippetCache>().FindByBottle(snippetName, project.BottleName) ??
                          snippets.Find(snippetName);

            return page.CodeSnippet(snippet);
        }


        public static IHtmlString ProjectSummary(this IFubuPage page)
        {
            var project = page.Get<ITopicContext>().Project;
            return page.Partial(project);
        }

        public static HtmlTag TableOfContents(this IFubuPage page)
        {
            return page.Get<TopicTreeBuilder>().BuildTableOfContents();
        }

        public static TagList LeftTopicNavigation(this IFubuPage page)
        {
            return page.Get<TopicTreeBuilder>().BuildLeftTopicLinks().ToTagList();
        }

        public static TagList TopTopicNavigation(this IFubuPage page)
        {
            return page.Get<TopicTreeBuilder>().BuildTopTopicLinks().ToTagList();
        }

        public static HtmlTag LinkToTopic(this IFubuPage page, string name, string title)
        {
            var context = page.Get<ITopicContext>();
            Topic topic = context.Project.FindByKey(name);
            if (topic == null)
            {
                var available = context.Project.AllTopics().Select(x => "'" + x.Key + "'").Join(", \n");

                throw new ArgumentOutOfRangeException("name", "Topic '{0}' cannot be found.  Try:\n{1}".ToFormat(name, available));
            }

            return new TopicLinkTag(page.Get<ICurrentHttpRequest>(), topic, title);
        }

        public static HtmlTag LinkToExternalTopic(this IFubuPage page, string name, string title)
        {
            Topic topic = TopicGraph.AllTopics.Find(name);
            if (topic == null)
            {
                return new HtmlTag("span").Text("*LINK TO " + name + "*");
            }

            return new TopicLinkTag(page.Get<ICurrentHttpRequest>(), topic, title);
        }


        public static HtmlTag ProjectLink(this IFubuPage page, string name)
        {
            var project = TopicGraph.AllTopics.TryFindProject(name);
            if (project == null)
            {
                return new HtmlTag("span").Text("LINK TO PROJECT '{0}'".ToFormat(name));
            }

            return new LinkTag(project.Name, page.Get<ICurrentHttpRequest>().ToRelativeUrl(project.Home.AbsoluteUrl)).Attr("title", project.Description);
        }

        public static string ProjectIndexUrl(this IFubuPage page, string name)
        {
            var project = TopicGraph.AllTopics.TryFindProject(name);
            if (project == null)
            {
                return "#";
            }

            return page.Get<ICurrentHttpRequest>().ToRelativeUrl(project.Index.AbsoluteUrl);
        }

        public static HtmlTag RootLink(this IFubuPage page)
        {
            var root = page.Get<IUrlRegistry>().UrlFor<AllTopicsEndpoint>(x => x.get_topics());
            return new HtmlTag("a")
                .Attr("href", root)
                .Attr("title", FubuDocsKeys.Fubu)
                .Append("span", span => span.Text(FubuDocsKeys.Fubu));
        }

        public static HtmlTag ProjectLogo(this IFubuPage page)
        {
            var project = page.Get<ITopicContext>().Project;
            if (project == null)
            {
                return new HtmlTag("div").Render(false);
            }

            // TODO -- Maybe include the project logo if it's specified?
            var homeUrl = page.Get<ICurrentHttpRequest>().ToRelativeUrl(project.Home.AbsoluteUrl);
            return new HtmlTag("a")
                .Attr("href", homeUrl)
                .Attr("title", project.TagLine)
                .AddClass("project-logo")
                .Append("span", span => span.Text(project.Name));
        }

        public static HtmlTag MailingList(this IFubuPage page, string text)
        {
            var project = page.Get<ITopicContext>().Project;

            if (project.UserGroupUrl.IsEmpty()) return LiteralTag.Empty();

            return new HtmlTag("em")
                    .Append("a", a => a.Attr("href", project.UserGroupUrl).Text(text));
        }
    }
}