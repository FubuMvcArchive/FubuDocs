using FubuLocalization;

namespace FubuDocs
{
    public class FubuDocsKeys : StringToken
    {
		public static readonly FubuDocsKeys Main = new FubuDocsKeys("Main");
        public static readonly FubuDocsKeys Fubu = new FubuDocsKeys("Fubu");

        private FubuDocsKeys(string defaultValue)
            : base(null, defaultValue, namespaceByType: true)
        {
        }
    }
}