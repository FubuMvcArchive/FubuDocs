using System;
using System.IO;
using System.Linq;
using System.Net;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Util.TextWriting;
using FubuDocs.Topics;
using FubuMVC.Core.Runtime;
using FubuMVC.Katana;
using System.Collections.Generic;

namespace FubuDocsRunner.Exports
{
    public class ContentUrl : IComparable<ContentUrl>
    {
        private static readonly IFileSystem fileSystem = new FileSystem();

        private readonly string _relativePath;

        public ContentUrl(string relativePath)
        {
            if (relativePath.StartsWith("../"))
            {
                throw new Exception("Shouldn't be doing this");
            }

            if (relativePath.StartsWith("http://"))
            {
                var segments = new Uri(relativePath).Segments;
                _relativePath = segments.Where(x => x != "/").Join("/");
            }
            else
            {
                _relativePath = relativePath.TrimStart('/');
            }

            if (_relativePath.EndsWith("index.html"))
            {
                _relativePath = _relativePath.ParentUrl();
            }
        }

        public override string ToString()
        {
            return string.Format("RelativePath: {0}", RelativePath);
        }

        public string RelativePath
        {
            get { return _relativePath; }
        }

        public void Write(EmbeddedFubuMvcServer server, ExportInput input, TextReport report)
        {
            var localPath = ToLocalPath(input.Output);
            var localDirectory = localPath.ParentDirectory();

            try
            {
                fileSystem.CreateDirectory(localDirectory); // Just making sure
                
                var client = new WebClient();
                var url = server.BaseAddress.AppendUrl(RelativePath);
                client.DownloadFile(url, localPath);

                report.AddColumnData(RelativePath, localPath);
            }
            catch (Exception e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Failed to write {0} to {1}".ToFormat(RelativePath, localPath));
                Console.WriteLine(e);

                throw;
            }
        }

        public bool IsBinary()
        {
            if (!Path.HasExtension(RelativePath)) return false;

            var mimeType = MimeType.MimeTypeByFileName(RelativePath);
            if (mimeType == MimeType.Javascript) return false;
            if (mimeType == MimeType.Css) return false;

            return true;
        }

        public string ToLocalPath(string root)
        {
            // if _content is in here, do it differently
            if (_relativePath.Contains("_content"))
            {
                return root.AppendPath(_relativePath.Split('/'));
            }

            return root.AppendPath(_relativePath.Split('/')).AppendPath("index.html");
        }

        public int Depth
        {
            get { 
                if (_relativePath.IsEmpty()) return 0;

                return _relativePath.Split('/').Count();
            }
        }

        public int CompareTo(ContentUrl other)
        {
            if (Depth != other.Depth)
            {
                return Depth.CompareTo(other.Depth);
            }

            return _relativePath.CompareTo(other._relativePath);
        }
    }
}