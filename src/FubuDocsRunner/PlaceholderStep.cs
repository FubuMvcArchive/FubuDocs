using FubuCore;

namespace FubuDocsRunner
{
    public class PlaceholderStep : IStep
    {
        private readonly string _directory;
        private string _file;

        public PlaceholderStep(string directory)
        {
            _directory = directory;
            _file = _directory.AppendPath("readme.txt");
        }

        public string Description()
        {
            return "Write file " + _file;
        }

        public bool Execute()
        {
            new FileSystem().WriteStringToFile(_file, "Replace this file w/ real contents!");
            return true;
        }
    }
}