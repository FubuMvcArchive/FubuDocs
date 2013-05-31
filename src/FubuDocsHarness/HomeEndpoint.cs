using FubuDocs.Tools;
using FubuMVC.Core.Continuations;

namespace FubuDocsHarness
{
    public class HomeEndpoint
    {
        public FubuContinuation Index()
        {
            return FubuContinuation.RedirectTo("fubudocs");
        }
    }
}