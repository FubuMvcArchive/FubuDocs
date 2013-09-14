using System;
using System.ComponentModel;
using System.Reflection;
using Fubu.Running;
using FubuCore;
using FubuDocs;

namespace FubuDocsRunner.Running
{
    public class RunInput
    {
        public RunInput()
        {
            BuildFlag = "Debug";
            BrowserFlag = BrowserType.Firefox;
        }

        [Description("Disables the bottle and code snippet scanning while this command runs")]
        public bool NoBottlingFlag { get; set; }

        [Description("Open a 'watched' browser with WebDriver to refresh the page on content or application recycles")]
        public bool WatchedFlag { get; set; }

        [Description("Browser to use in watched.  Default is Firefox because it's more stable. ")]
        public BrowserType BrowserFlag { get; set; }

        [Description("If you are running a class library, sets the preference for the profile to load.  As in bin/[BuildFlag].  Default is debug")]
        public string BuildFlag { get; set; }


        [Description("Start the default browser to the home page of this application")]
        public bool OpenFlag { get; set; }

        [Description("Specify the directory for a hosting application for your documentation.  Will be loaded as a FubuMVC Bottle")]
        public string HostFlag { get; set; }



        public ApplicationRequest ToRequest()
        {
            return new ApplicationRequest
            {
                ApplicationFlag = typeof (FubuDocsApplication).Name,
                DirectoryFlag = HostFlag ?? Assembly.GetExecutingAssembly().Location.ParentDirectory(),
                WatchedFlag = WatchedFlag,
                BrowserFlag = BrowserFlag,
                BuildFlag = BuildFlag,
                OpenFlag = OpenFlag
            };
        }

        public virtual FubuDocsDirectories ToDirectories()
        {
            return new FubuDocsDirectories
            {
                Host = HostFlag,
                Solution = Environment.CurrentDirectory,
                RootUrls = true
            };
        }
    }
}