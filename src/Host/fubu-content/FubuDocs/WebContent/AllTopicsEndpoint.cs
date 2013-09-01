using FubuDocs.Exporting;
using FubuMVC.Core.Http;

namespace FubuDocs
{
    [Export]
    public class AllTopicsEndpoint
    {
        private readonly ICurrentHttpRequest _request;

        public AllTopicsEndpoint(ICurrentHttpRequest request)
        {
            _request = request;
        }

        public AllProjectsModel get_topics()
        {
            return new AllProjectsModel(_request);
        }
    }
}