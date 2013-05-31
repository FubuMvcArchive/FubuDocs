using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuDocsRunner.Topics
{
    public class BatchAdderController
    {
        private readonly ITopicFileSystem _fileSystem;
        private readonly IList<TopicToken> _topics = new List<TopicToken>(); 

        public BatchAdderController(ITopicFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Start()
        {
            _topics.Clear();
            _topics.AddRange(_fileSystem.ReadTopics().OrderBy(x => x.Order));

            for (int i = 0; i < _topics.Count; i++)
            {
                TopicToken topicToken = (TopicToken) _topics[i];
                var order = i + 1;
                if (topicToken.Order != order)
                {
                    _fileSystem.Reorder(topicToken, order);
                }
            }
        }

        public WhatNext ReadText(string text)
        {
            if (text == null) return WhatNext.Stop;

            text = text.Trim();

            if (text.IsEmpty()) return WhatNext.Stop;
            
            if (text.EqualsIgnoreCase("q") || text.EqualsIgnoreCase("quit")) return WhatNext.Stop;

            var token = TopicToken.Read(text);
            token.Order = _topics.Count + 1;

            _fileSystem.AddTopic(token);

            _topics.Add(token);

            return WhatNext.ReadMore;
        }
    }
}