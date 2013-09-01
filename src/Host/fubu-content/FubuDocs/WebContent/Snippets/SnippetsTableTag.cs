using System.Collections.Generic;
using System.Linq;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuDocs.Snippets
{
    public class SnippetsTableTag : TableTag
    {
        public SnippetsTableTag(IUrlRegistry urls, IEnumerable<Snippet> snippets)
        {
            AddClass("table");

            AddHeaderRow(tr => {
                tr.Header("Name");
                tr.Header("Bottle");
                tr.Header("File");
            });

            snippets.OrderBy(x => x.BottleName).ThenBy(x => x.Name).Each(snippet => {
                AddBodyRow(tr => showSnippet(tr, snippet, urls));
            });
        }

        private void showSnippet(TableRowTag tr, Snippet snippet, IUrlRegistry urls)
        {
            var request = new SnippetRequest(snippet);
            var url = urls.UrlFor(request, "GET");
            tr.Cell().Add("a").Attr("href", "#").Data("url", url).AddClass("snippet-link").Text(snippet.Name);
            tr.Cell(snippet.BottleName);


            var editUrl = urls.UrlFor(request, "POST");
            tr.Cell().Add("a").Data("url", editUrl)
              .Data("name", snippet.Name)
              .Data("bottle", snippet.BottleName)
              .Attr("href", "#").AddClass("edit-snippet").Text(snippet.File);

        }
    }
}