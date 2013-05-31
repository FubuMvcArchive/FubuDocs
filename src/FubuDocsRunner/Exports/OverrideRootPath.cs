using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;

namespace FubuDocsRunner.Exports
{
    public class DisplayDownloadReport : IDownloadReportVisitor
    {
        public void Visit(DownloadReport report)
        {
            Console.WriteLine(report.ToDescriptionText());
        }
    }

    public class OverrideRootPath : IDownloadReportVisitor
    {
        private readonly string _root;
        private readonly IFileSystem _fileSystem;

        private static readonly string[] Excludes = new[]
        {
            "/", "/_content/styles", "/a"
        };

        public OverrideRootPath(string root)
        {
            _root = root;
            _fileSystem = new FileSystem();
        }

        public void Visit(DownloadReport report)
        {
            var files = new List<string>();
            
            report
                .Reverse()
                .Each(item => makeRelative(report, item, files.Add));

            files.Each(file =>
            {
                var contents = _fileSystem.ReadStringFromFile(file);
                var replaced = replaceFileContents(report, contents);

                _fileSystem.WriteStringToFile(file, replaced);
            });

            removeEmptyDirectories(report);
        }

        private void removeEmptyDirectories(IEnumerable<ItemDownloaded> report)
        {
            report.Each(item =>
            {
                var dir = item.OutputDir.AppendPath(item.Token.RootFolder);
                if (!Directory.Exists(dir))
                {
                    return;
                }

                var files = new DirectoryInfo(dir).GetFiles("*.*", SearchOption.AllDirectories);
                var empty = !files.Any();
                if (empty)
                {
                    Directory.Delete(dir, true);
                }
            });
        }

        private void makeRelative(DownloadReport report, ItemDownloaded item, Action<string> callback)
        {
            var token = item.Token;
            if (!_fileSystem.FileExists(item.FullPath))
            {
                return;
            }

            var relative = token.RelativeAt(_root);
            var current = Environment.CurrentDirectory;
            var target = relative.GetLocalPath(current.AppendPath(item.OutputDir));

            if (_fileSystem.FileExists(target))
            {
                return;
            }

            _fileSystem.MoveFile(item.FullPath, target);

            if (relative.IsModifiable())
            {
                callback(target);
            }
        }

        private string replaceFileContents(IEnumerable<ItemDownloaded> items, string contents)
        {
            var srcExtensions = new[] {"js", "jpg", "jpeg", "gif", "png"};
            items
                .Each(item =>
                {
                    var token = item.Token;
                    if(Excludes.Contains(token.RelativeUrl)) return;

                    var relative = token.RelativeAt(_root);
                    var local = token.LocalPath.ToLower();

                    if (local.EndsWith(".html") || local.EndsWith(".css"))
                    {
                        contents = replace(contents, "href", token.RelativeUrl, relative.RelativeUrl);
                    }
                    else if (srcExtensions.Any(local.EndsWith))
                    {
                        contents = replace(contents, "src", token.RelativeUrl, relative.RelativeUrl);
                    }
                });

            return contents;
        }

        private static string replace(string contents, string key, string url1, string url2)
        {
            var token1 = attribute(key, url1, "\"");
            var token2 = attribute(key, url2, "\"");

            var replacedContents = contents.Replace(token1, token2);

            token1 = attribute(key, url1, "'");
            token2 = attribute(key, url2, "'");

            return replacedContents.Replace(token1, token2);
        }

        private static string attribute(string key, string url, string quote)
        {
            return "{0}={1}{2}{1}".ToFormat(key, quote, url);
        }
    }
}