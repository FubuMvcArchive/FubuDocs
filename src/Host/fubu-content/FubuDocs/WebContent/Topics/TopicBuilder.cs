using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuDocs.Topics
{
    public static class TopicBuilder
    {
        private static FileSystem FileSystem = new FileSystem();

        public static string FindTitle(string filePath)
        {
            Func<string, bool> filter = x => x.StartsWith("Title:", StringComparison.OrdinalIgnoreCase);
            IEnumerable<string> comments = findComments(filePath).ToArray();
            var rawTitle = comments.FirstOrDefault(filter);
            if (rawTitle != null)
            {
                var title = rawTitle.Split(':').Last().Trim();
                return title;
            } 

            return null;
        }

        public static void BuildOut(Topic topic)
        {
            topic.Url = topic.Key;

            var filePath = topic.File.FilePath;
            if (FileSystem.FileExists(filePath))
            {
                Func<string, bool> filter = x => x.StartsWith("Title:", StringComparison.OrdinalIgnoreCase);
                IEnumerable<string> comments = findComments(filePath).ToArray();
                var rawTitle = comments.FirstOrDefault(filter);
                if (rawTitle != null)
                {
                    var title = rawTitle.Split(':').Last().Trim();
                    topic.Title = title;
                }

                if (!topic.IsIndex)
                {
                    var rawUrl = comments.FirstOrDefault(x => x.StartsWith("Url:", StringComparison.OrdinalIgnoreCase));
                    if (rawUrl.IsNotEmpty())
                    {
                        var segment = rawUrl.Split(':').Last().Trim();
                        topic.Url = topic.Url.ParentUrl().AppendUrl(segment);
                    }
                }

                var import = comments.FirstOrDefault(x => x.StartsWith("Import:", StringComparison.OrdinalIgnoreCase));
                if (import != null)
                {
                    topic.Import = import.Split(':').Last().Trim().ToLower();
                }
            }

            if (topic.Title.IsEmpty())
            {
                topic.Title = topic.Name.Capitalize().SplitPascalCase();
            }
        }

        private static IEnumerable<string> findComments(string file)
        {
            var regex = @"<!--(.*?)-->";
            var matches = Regex.Matches(FileSystem.ReadStringFromFile(file), regex);
            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value.Trim();
            }
        }
    }
}