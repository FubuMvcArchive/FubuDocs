using System;
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
                Id = Guid.Empty.ToString()
            });

            TopicTemplate.Id("new-leaf");
        }

        public string Name { get; set; }
        public ProjectRoot Project { get; set; }

        public string SubmitUrl { get; set; }
        public HtmlTag TopicTemplate { get; set; }
        public HtmlTag Topics { get; set; }
        public HtmlTag Files { get; set; }
        public HtmlTag Snippets { get; set; }
        public HtmlTag TodoList { get; set; }
    }
}