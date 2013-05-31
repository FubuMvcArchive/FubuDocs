using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuDocs.Topics
{
    public class TopicFolder : OrderedTopic, ITopicNode
    {
        private readonly IList<OrderedTopic> _children = new List<OrderedTopic>();
        private readonly ProjectRoot _project;
        private readonly string _url;
        private Topic _root;

        public TopicFolder(string rawName, ProjectRoot project) : base(rawName.Split('/').Last())
        {
            _project = project;

            _url = rawName.Split('/').Select(FindValue).Join("/");
            _url = project.Url.AppendUrl(_url);
        }

        public string Url
        {
            get { return _url; }
        }

        public ProjectRoot Project
        {
            get { return _project; }
        }

        public void AddFiles(IEnumerable<Topic> topics)
        {
            _root = topics.FirstOrDefault(x => x.Name.EqualsIgnoreCase(Topic.Index));

            var others = topics.Where(x => !x.Name.EqualsIgnoreCase(Topic.Index));

            _children.AddRange(others);
        }

        public override IEnumerable<Topic> TopLevelTopics()
        {
            var orderedTopics = _children.SelectMany(x => x.TopLevelTopics()).ToList();
            orderedTopics.Sort();
            
            if (_root == null)
            {
                return orderedTopics;
            }

            orderedTopics.Each(x => _root.AppendChild(x));

            return new[] {_root};
        }

        public void Add(OrderedTopic orderedTopic)
        {
            _children.Add(orderedTopic);
        }

        public override string ToString()
        {
            return string.Format("Folder Url: {0}", _url);
        }
    }
}