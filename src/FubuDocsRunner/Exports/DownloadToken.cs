using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuDocsRunner.Exports
{
    public class DownloadToken
    {
        private static readonly object Lock = new object();

        private readonly FileSystem _fileSystem;

        private DownloadToken(string url, string[] parts, string localPath)
        {
            Url = url;
            Parts = parts;
            LocalPath = localPath;

            _fileSystem = new FileSystem();
        }

        public string BaseUrl { get; private set; }
        public string Url { get; private set; }
        public string[] Parts { get; private set; }
        public string LocalPath { get; private set; }
        public bool IsAsset { get; private set; }

        public string RootFolder { get { return Parts[0]; } }

        public string RelativeUrl
        {
            get
            {
                var url = Url.Replace(BaseUrl, "");
                return "/" + url.TrimStart('/');
            }
        }

        public string GetLocalPath(string directory)
        {
            lock (Lock)
            {
                if (!_fileSystem.DirectoryExists(directory))
                {
                    _fileSystem.CreateDirectory(directory);
                }
            }

            var attempts = new List<string>();
            Parts.Each(x =>
            {
                attempts.Add(x);
                lock (Lock)
                {
                    var file = directory;
                    attempts.Each(y => file = file.AppendPath(y));

                    var isFile = attempts.Count == Parts.Length && Path.HasExtension(file);
                    if (!_fileSystem.FileExists(file) && !isFile)
                    {
                        _fileSystem.CreateDirectory(file);
                    }
                }
            });

            return directory.AppendPath(LocalPath);
        }

        protected bool Equals(DownloadToken other)
        {
            return Url.EqualsIgnoreCase(other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DownloadToken)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Url.GetHashCode() * 397) ^ LocalPath.GetHashCode();
            }
        }

        public override string ToString()
        {
            return Url;
        }

        public static DownloadToken For(string baseUrl, string relativePath)
        {
            baseUrl = baseUrl.TrimEnd('/');
            relativePath = relativePath.Replace(baseUrl, "");

            //var isAsset = true;
            var url = baseUrl + relativePath;
            //var lastIndex = relativePath.LastIndexOf('.');

            // TODO -- This is naive. We might need to make the execution deferred and go off the mime type
            var isAsset = relativePath.Contains("_content");

            if (!isAsset)
            {
                relativePath += "/index.html";
            }

            var parts = relativePath.TrimStart('/').Split(new [] { "/" }, StringSplitOptions.None);

            var path = "";
            parts.Each(part => path = path.AppendPath(part));

            return new DownloadToken(url, parts, path)
            {
                BaseUrl = baseUrl,
                IsAsset = isAsset
            };
        }

        public DownloadToken RelativeAt(string root)
        {
            if (!root.StartsWith("/") && !root.StartsWith("http"))
            {
                root = "/" + root;
            }

            var relative = root.TrimEnd('/') + RelativeUrl;
            return For(BaseUrl, relative);
        }

        public bool IsRelativeAt(string path)
        {
            return RelativeUrl.StartsWith(path);
        }

        public bool IsModifiable()
        {
            return LocalPath.ToLower().EndsWith(".html");
        }
    }
}