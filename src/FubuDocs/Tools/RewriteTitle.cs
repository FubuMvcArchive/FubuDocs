using System;
using System.Linq;
using FubuCore;
using FubuDocs.Topics;

namespace FubuDocs.Tools
{
    public class RewriteTitle : IDelta
    {
        private readonly string _file;
        private readonly string _title;

        public RewriteTitle(string file, string title)
        {
            _file = file;
            _title = title;
        }

        public void Prepare()
        {
            
        }

        public void Execute()
        {
            var contents = TopicToken.FileSystem.ReadStringFromFile(_file);
            var comments = TopicBuilder.FindComments(contents);

            var existingTitle = comments.FirstOrDefault(x => x.StartsWith("Title"));
            if (existingTitle == null)
            {
                contents = "<!-- Title: {0} -->".ToFormat(_title) + Environment.NewLine + contents;
            }
            else
            {
                contents = contents.Replace(existingTitle, "Title: " + _title);
            }

            TopicToken.FileSystem.WriteStringToFile(_file, contents);
        }

        protected bool Equals(RewriteTitle other)
        {
            return string.Equals(_file, other._file) && string.Equals(_title, other._title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RewriteTitle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_file != null ? _file.GetHashCode() : 0)*397) ^ (_title != null ? _title.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Change title of file {0} to '{1}'", _file, _title);
        }
    }
}