using System;
using System.IO;
using FubuCore.Util;
using FubuDocsRunner.Exports;

namespace FubuDocs.Tests.Exports
{
    public class StubDownloader : IDownloader
    {
        private readonly Cache<string, string> _cache = new Cache<string, string>();

        public void Add(string url, string contents)
        {
            _cache.Fill(url, contents);
        }

        public string ContentsFor(string url)
        {
            return _cache[url];
        }

        public void Download(string url, string filePath, Action<string> continuation)
        {
            using (var writer = new StreamWriter(filePath, false))
            {
                var contents = ContentsFor(url);
                
                writer.Write(contents);
                writer.Flush();

                continuation(contents);
            }
        }
    }
}