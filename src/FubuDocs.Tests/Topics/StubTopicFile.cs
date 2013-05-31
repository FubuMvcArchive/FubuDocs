using System;
using System.IO;
using FubuCore;
using FubuDocs.Topics;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.View;

namespace FubuDocs.Tests.Topics
{
    public class StubTopicFile : ITopicFile
    {
        public StubTopicFile()
        {
            Name = Guid.NewGuid().ToString() + ".spark";
            FilePath = Name;
            Folder = Path.GetFileNameWithoutExtension(Name);
        }

        public StubTopicFile(string file)
        {
            FilePath = file.ToFullPath();
        }

        public string FilePath { get; set; }
        public string Name { get; set; }

        public string Folder { get; set; }

        public IViewToken ToViewToken()
        {
            return new StubViewToken(this);
        }

        public void WriteContents(string contents)
        {
            new FileSystem().WriteStringToFile(FilePath, contents);
        }
    }

    public class StubViewToken : IViewToken
    {
        private readonly StubTopicFile _file;

        public StubViewToken(StubTopicFile file)
        {
            _file = file;
        }

        public string Name()
        {
            return _file.Name;
        }

        public ObjectDef ToViewFactoryObjectDef()
        {
            throw new NotImplementedException();
        }

        public Type ViewType { get; private set; }
        public Type ViewModel { get; private set; }
        public string Namespace { get; private set; }
        public string ProfileName { get; set; }

        protected bool Equals(StubViewToken other)
        {
            return Equals(_file, other._file);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StubViewToken) obj);
        }

        public override int GetHashCode()
        {
            return (_file != null ? _file.GetHashCode() : 0);
        }
    }
}