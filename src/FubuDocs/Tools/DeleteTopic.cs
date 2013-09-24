using System;

namespace FubuDocs.Tools
{
    public class DeleteTopic : IDelta
    {
        private readonly string _file;

        public DeleteTopic(TopicToken token)
        {
            _file = token.File;
        }

        public void Prepare()
        {
            Console.WriteLine("Deleting " + _file);
            TopicToken.FileSystem.DeleteFile(_file);
        }

        public void Execute()
        {
            
        }

        public string File
        {
            get { return _file; }
        }

        protected bool Equals(DeleteTopic other)
        {
            return string.Equals(_file, other._file);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeleteTopic) obj);
        }

        public override int GetHashCode()
        {
            return (_file != null ? _file.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Delete file at {0}", _file);
        }
    }
}