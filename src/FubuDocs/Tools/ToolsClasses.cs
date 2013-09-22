using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuDocs.Topics;

namespace FubuDocs.Tools
{
    public class TopicToken
    {
        private readonly IList<TopicToken> Children = new List<TopicToken>();

        public TopicToken(Topic topic)
        {
            Key = topic.Key;
            Title = topic.Title;
            Url = topic.Url;
            File = topic.File.FilePath;

            // Need to go down here
            
        }

        public string File;


        public Guid Id = Guid.NewGuid();
        public string Key;
        public string Title;
        public string Url;

    }

    public class TopicFileSystem
    {
        private readonly string _directory;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public TopicFileSystem(string directory)
        {
            _directory = directory;
        }


        public string WriteFile(TopicToken token, int order)
        {
            return writeNewFile(token, order);
        }


        private string writeNewFile(TopicToken token, int order)
        {
            var filename = "{0}.{1}.spark".ToFormat(order, token.Key);
            var path = _directory.AppendPath(filename);

            return writeTokenToFile(token, path);
        }

        private string writeTokenToFile(TopicToken token, string path)
        {
            _fileSystem.AlterFlatFile(path, list =>
            {
                list.Add("<!--Title: {0}-->".ToFormat(token.Title));
                list.Add("");
                list.Add("<markdown>");
                list.Add("TODO(Write content!)");
                list.Add("</markdown>");
                list.Add("");
            });

            return path;
        }
    }

    
}