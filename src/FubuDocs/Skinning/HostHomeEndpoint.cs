using FubuMVC.Core;

namespace FubuDocs.Skinning
{
    public class HostHomeEndpoint
    {
        [FubuPartial]
        public HostHome Render()
        {
            return new HostHome();
        }
    }

    public class HostHome
    {
    }
}