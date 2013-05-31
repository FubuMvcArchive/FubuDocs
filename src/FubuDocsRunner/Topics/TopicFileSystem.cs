using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuDocs.Topics;

namespace FubuDocsRunner.Topics
{
    public class TopicFileSystem : ITopicFileSystem
    {
        private readonly string _directory;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public TopicFileSystem(string directory)
        {
            _directory = directory;
        }

        public IEnumerable<TopicToken> ReadTopics()
        {
            var files = _fileSystem.FindFiles(_directory, FileSet.Shallow("*.spark", "index.spark"))
                                   .Select(readFile);

            var folders = Directory.GetDirectories(_directory, "*", SearchOption.TopDirectoryOnly)
                .Where(x => !Path.GetFileName(x).EqualsIgnoreCase("samples"))
                .Where(x => !Path.GetFileName(x).EqualsIgnoreCase("examples"))
                .Where(x => Path.GetDirectoryName(x) != "bin")
                .Where(x => Path.GetDirectoryName(x) != "obj")
                .Where(x => _fileSystem.FindFiles(x, FileSet.Shallow("*.spark")).Any())
                                   .Select(readDirectory);

            return files.Union(folders).OrderBy(x => x.Order);
        }

        private TopicToken readDirectory(string directory)
        {
            var indexPath = directory.AppendPath("index.spark");
            if (!_fileSystem.FileExists(indexPath))
            {
                throw new NotSupportedException("You can only use batch topic actions on child folders with an index.spark file");
            }

            var keyParts = Path.GetFileName(directory).Split('.');
            return new TopicToken
            {
                Key = keyParts.Last(),
                Order = keyParts.Length > 1 ? int.Parse(keyParts.First()) : 0,
                Type = TopicTokenType.Folder,
                RelativePath = directory.PathRelativeTo(_directory),
                Title = TopicBuilder.FindTitle(indexPath) ?? keyParts.Last()
            };
        }

        private TopicToken readFile(string file)
        {
            var keyParts = Path.GetFileNameWithoutExtension(file).Split('.');
            return new TopicToken
            {
                Key = keyParts.Last(),
                Order = keyParts.Length > 1 ? int.Parse(keyParts.First()) : 0,
                Type = TopicTokenType.File,
                RelativePath = file.PathRelativeTo(_directory),
                Title = TopicBuilder.FindTitle(file) ?? keyParts.Last()
            };
        }

        public void AddTopic(TopicToken token)
        {
            WriteFile(token);
        }

        public void Reorder(TopicToken topicToken, int order)
        {
            var newFile = "{0}.{1}.spark".ToFormat(order, topicToken.Key);
            _fileSystem.MoveFile(_directory.AppendPath(topicToken.RelativePath), _directory.AppendPath(newFile));

            topicToken.Order = order;
        }

        public string WriteFile(TopicToken token)
        {
            if (token.Type == TopicTokenType.File)
            {
                return writeNewFile(token);
            }
            else
            {
                return writeNewFolder(token);
            }
        }

        private string writeNewFolder(TopicToken token)
        {
            var childFolder = _directory.AppendPath("{0}.{1}".ToFormat(token.Order, token.Key));
            _fileSystem.CreateDirectory(childFolder);

            var path = childFolder.AppendPath("index.spark");

            writeTokenToFile(token, path);

            return path;
        }

        private string writeNewFile(TopicToken token)
        {
            var filename = "{0}.{1}.spark".ToFormat(token.Order, token.Key);
            var path = _directory.AppendPath(filename);

            return writeTokenToFile(token, path);
        }

        private string writeTokenToFile(TopicToken token, string path)
        {
            _fileSystem.AlterFlatFile(path, list => {
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