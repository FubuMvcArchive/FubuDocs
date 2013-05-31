using FubuLocalization;

namespace FubuDocs
{
    public class FubuDocsKeys : StringToken
    {
        public static readonly FubuDocsKeys Fubu = new FubuDocsKeys("Fubu");
        public static readonly FubuDocsKeys MailingList = new FubuDocsKeys("Join our vibrant mailing list");

        private FubuDocsKeys(string defaultValue)
            : base(null, defaultValue, namespaceByType: true)
        {
        }
    }
}