using System.ComponentModel;
using System.Reflection;
using Fubu.Running;
using FubuCore;

namespace FubuDocsRunner.Running
{
    public class RunInput : ApplicationRequest
    {
        public RunInput()
        {
            ApplicationFlag = typeof (FubuDocsApplication).Name;
            DirectoryFlag = Assembly.GetExecutingAssembly().Location.ParentDirectory();
        }

        [Description("Disables the bottle and code snippet scanning while this command runs")]
        public bool NoBottlingFlag { get; set; }

        [Description("If set, the relative starting url at which to open the browser")]
        public string UrlFlag { get; set; }
    }
}