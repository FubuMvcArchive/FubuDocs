using System;
using System.Reflection;
using FubuCore.Binding;
using FubuDocs.Topics;
using HtmlTags;

namespace FubuDocs.Tools
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            TopicTemplate = new TopicListItemTag(new TopicToken
            {
                Title = "New Topic",
                Url = string.Empty,
                Key = string.Empty,
                Id = Guid.Empty.ToString(),
                
            });

            TopicTemplate.AddClass("new-topic");
            TopicTemplate.Id("new-leaf");
        }

        public string Name { get; set; }
        public ProjectRoot Project { get; set; }

        [DoNothing]
        public string SubmitUrl { get; set; }
        [DoNothing]
        public HtmlTag TopicTemplate { get; set; }
        [DoNothing]
        public HtmlTag Topics { get; set; }
        [DoNothing]
        public HtmlTag Files { get; set; }
        [DoNothing]
        public HtmlTag Snippets { get; set; }
        [DoNothing]public HtmlTag TodoList { get; set; }
    }

    public class DoNothingAttribute : BindingAttribute
    {
        public override void Bind(PropertyInfo property, IBindingContext context)
        {
            // do nothing
        }
    }
}