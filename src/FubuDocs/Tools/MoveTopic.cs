using System;
using FubuCore;

namespace FubuDocs.Tools
{
    public class MoveTopic : IDelta
    {
        private readonly string _original;
        private readonly string _moved;
        private string _contents;

        public MoveTopic(string original, string moved)
        {
            if (moved.IsEmpty())
            {
                throw new ArgumentOutOfRangeException("moved", "'moved' cannot be null or empty");
            }

            _original = original;
            _moved = moved;
        }

        public void Prepare()
        {
            _contents = TopicToken.FileSystem.ReadStringFromFile(_original);
            TopicToken.FileSystem.DeleteFile(_original);
        }

        public void Execute()
        {
            TopicToken.FileSystem.WriteStringToFile(_moved, _contents);
        }

        public override string ToString()
        {
            return string.Format("Move {0} to {1}", _original, _moved);
        }

        protected bool Equals(MoveTopic other)
        {
            return string.Equals(_original, other._original) && string.Equals(_moved, other._moved);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MoveTopic) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_original != null ? _original.GetHashCode() : 0)*397) ^ (_moved != null ? _moved.GetHashCode() : 0);
            }
        }
    }
}