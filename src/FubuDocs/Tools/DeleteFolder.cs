namespace FubuDocs.Tools
{
    public class DeleteFolder : IDelta
    {
        private readonly string _folder;

        public DeleteFolder(string folder)
        {
            _folder = folder;
        }

        public void Prepare()
        {
            
        }

        public void Execute()
        {
            TopicToken.FileSystem.DeleteDirectory(_folder);
        }

        public override string ToString()
        {
            return string.Format("Delete folder {0}", _folder);
        }
    }
}