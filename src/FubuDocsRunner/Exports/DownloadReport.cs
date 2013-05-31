using System.Collections;
using System.Collections.Generic;
using FubuCore.Descriptions;

namespace FubuDocsRunner.Exports
{
    public class DownloadReport : IEnumerable<ItemDownloaded>, DescribesItself
    {
        private readonly IList<ItemDownloaded> _downloads = new List<ItemDownloaded>();

        public void ItemDownloaded(ItemDownloaded download)
        {
            _downloads.Fill(download);
        }

        public void Describe(Description description)
        {
            description.AddList("Downloads", _downloads);
        }

        public IEnumerator<ItemDownloaded> GetEnumerator()
        {
            return _downloads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}