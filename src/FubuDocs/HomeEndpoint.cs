using FubuDocs.Exporting;
using FubuMVC.Core.Http;

namespace FubuDocs
{
    [Export]
    public class AllTopicsEndpoint
    {
        private readonly ICurrentHttpRequest _request;
        private readonly FubuDocsDirectories _directories;

        public AllTopicsEndpoint(ICurrentHttpRequest request, FubuDocsDirectories directories)
        {
            _request = request;
            _directories = directories;
        }

        public AllProjectsModel get_topics()
        {
            return new AllProjectsModel(_request, _directories);
        }
    }
}