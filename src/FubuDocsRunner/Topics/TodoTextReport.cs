using System.Collections.Generic;
using FubuCore;
using FubuCore.Util.TextWriting;
using FubuDocs.Todos;
using FubuDocs.Topics;

namespace FubuDocsRunner.Topics
{
    public class TodoTextReport : TextReport
    {
        public TodoTextReport(string folder, IEnumerable<Topic> topics)
        {
            AddDivider('-');
            StartColumns(new Column(ColumnJustification.left, 0, 5),
                         new Column(ColumnJustification.right, 0, 5),
                         new Column(ColumnJustification.left, 0, 0)
                );            
            AddColumnData("File", "Line", "TODO");
            AddDivider('-');

            var todos = TodoTask.FindAllTodos(topics);

            todos.Each(todo => {
                AddColumnData(todo.File.PathRelativeTo(folder), todo.Line.ToString(), todo.Message);
            });
        }
    }
}