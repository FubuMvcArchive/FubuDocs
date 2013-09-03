using System.ComponentModel;

namespace FubuDocsRunner
{
    public class GhPagesInput
    {
        [Description("The reference to the git repo where you want the gh-pages branch")]
        public string GitRepository { get; set; }
    }
}