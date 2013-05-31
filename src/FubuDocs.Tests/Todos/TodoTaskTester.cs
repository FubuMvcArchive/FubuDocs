using System.Diagnostics;
using FubuCore;
using FubuDocs.Tests.Topics;
using FubuDocs.Todos;
using FubuDocs.Topics;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace FubuDocs.Tests.Todos
{
    [TestFixture]
    public class TodoTaskTester
    {
        

        [Test]
        public void build_a_single_task()
        {
            new FileSystem().WriteStringToFile("todos.txt", @"
nothing
nothing
nothing
nothing
nothing
else TODO(what to do here?) again
nothing
foo TODO(bar) and TODO(more)
no
what TODO here?
no
TODO last
");

            var topic = new Topic(new ProjectRoot{Name = "something", Url = "something"}, new StubTopicFile("todos.txt"));

            var tasks = TodoTask.ReadTasks(topic).ToArray();

            tasks.Each(x => Debug.WriteLine(x));

            tasks.ShouldHaveTheSameElementsAs(
                new TodoTask{File = topic.File.FilePath, Key = topic.Key, Line = 7, Message = "what to do here?"},
                new TodoTask{File = topic.File.FilePath, Key = topic.Key, Line = 9, Message = "bar"},
                new TodoTask{File = topic.File.FilePath, Key = topic.Key, Line = 9, Message = "more"},
                new TodoTask{File = topic.File.FilePath, Key = topic.Key, Line = 11, Message = "here?"},
                new TodoTask{File = topic.File.FilePath, Key = topic.Key, Line = 13, Message = "last"}
                
                
                );


        }
    }
}