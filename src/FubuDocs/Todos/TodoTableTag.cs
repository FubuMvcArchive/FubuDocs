using System.Collections.Generic;
using HtmlTags;

namespace FubuDocs.Todos
{
    public class TodoTableTag : TableTag
    {
        public TodoTableTag(string url, IEnumerable<TodoTask> todos)
        {
            AddClass("table");

            AddHeaderRow(tr => {
                tr.Header("File");
                tr.Header("Line");
                tr.Header("Message");
            });

            todos.Each(task => {
                AddBodyRow(tr => {
                    tr.Cell().Add("a").Add("a").Data("url", url).Data("key", task.Key).Attr("href", "#").AddClass("edit-link").Text(task.File);
                    tr.Cell(task.Line.ToString()).Style("align", "right");
                    tr.Cell(task.Message);
                });
            });
        }
    }
}