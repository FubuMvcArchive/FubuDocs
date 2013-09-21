using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Spark;
using FubuMVC.StructureMap;
using StructureMap;

namespace FubuDocsHarness
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            FubuMode.Mode(FubuMode.Development);

            FubuApplication.For<FubuDocsHarnessRegistry>().StructureMap(new Container()).Bootstrap();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

    public class FubuDocsHarnessRegistry : FubuRegistry
    {
        public FubuDocsHarnessRegistry()
        {
            AlterSettings<SparkEngineSettings>(x => x.PrecompileViews = false);
        }
    }

    public class HomeEndpoint
    {
        public FubuContinuation Index()
        {
            return FubuContinuation.RedirectTo("/project/fubudocs");
        }
    }
}