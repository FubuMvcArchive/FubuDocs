using System;
using System.Linq;
using FubuCore;
using FubuDocs.Topics;

namespace FubuDocs.Tools
{
    public class RewriteUrl : IDelta
    {
        private readonly string _file;
        private readonly string _url;

        public RewriteUrl(string file, string url)
        {
            _file = file;
            _url = url;
        }

        public void Prepare()
        {

        }

        public void Execute()
        {
            var contents = TopicToken.FileSystem.ReadStringFromFile(_file);
            var comments = TopicBuilder.FindComments(contents);

            var existingUrl = comments.FirstOrDefault(x => x.StartsWith("Url"));
            if (existingUrl == null)
            {
                contents = "<!-- Url: {0} -->".ToFormat(_url) + Environment.NewLine + contents;
            }
            else
            {
                contents = contents.Replace(existingUrl, "Url: " + _url);
            }

            TopicToken.FileSystem.WriteStringToFile(_file, contents);
        }

        protected bool Equals(RewriteUrl other)
        {
            return string.Equals(_file, other._file) && string.Equals(_url, other._url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RewriteUrl) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_file != null ? _file.GetHashCode() : 0)*397) ^ (_url != null ? _url.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return "Change url of file {0} to '{1}'".ToFormat(_file, _url);
        }
    }
}