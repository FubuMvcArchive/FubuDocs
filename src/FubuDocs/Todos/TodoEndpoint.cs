using System;
using FubuCore;
using FubuDocs.Navigation;
using System.Linq;
using FubuMVC.Core.Urls;

namespace FubuDocs.Todos
{
    public class TodoEndpoint
    {
        private readonly IUrlRegistry _urls;

        public TodoEndpoint(IUrlRegistry urls)
        {
            _urls = urls;
        }

        public TodoModel get_todos()
        {
            var url = _urls.UrlFor<FileRequest>();

            return new TodoModel
            {
                Tag = new TodoTableTag(url, TodoTask.FindAllTodos().OrderBy(x => x.File).ThenBy(x => x.Line))
            };
        }
    }
}