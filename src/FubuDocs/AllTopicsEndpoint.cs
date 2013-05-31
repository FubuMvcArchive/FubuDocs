using FubuDocs.Exporting;

namespace FubuDocs
{
    [Export]
    public class AllTopicsEndpoint
    {
        public AllProjectsModel get_topics()
        {
            return new AllProjectsModel();
        }
    }
}