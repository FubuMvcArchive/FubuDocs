namespace FubuDocs.Tools
{
    public class CreateFolder : IDelta
    {
        private readonly string _folder;

        public CreateFolder(string folder)
        {
            _folder = folder;
        }

        public string Folder
        {
            get
            {
                return _folder;
            }
        }

        public void Prepare()
        {
            TopicToken.FileSystem.CreateDirectory(_folder);
        }

        public void Execute()
        {
            
        }

        public override string ToString()
        {
            return string.Format("Create folder: {0}", _folder);
        }

        protected bool Equals(CreateFolder other)
        {
            return string.Equals(_folder, other._folder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CreateFolder) obj);
        }

        public override int GetHashCode()
        {
            return (_folder != null ? _folder.GetHashCode() : 0);
        }
    }
}