using System.Collections.Generic;
using System.Linq;
using FubuDocs.Exporting;

namespace FubuDocsRunner.Exports
{
    public class AccessedCache : IAccessedCache
    {
        private readonly IList<string> _visited = new List<string>(); 
        private readonly List<ContentUrl> _queued = new List<ContentUrl>();
 
        public void Enqueue(string url)
        {
            if (UrlQueryEndpoint.IgnoredPatterns.Any(x => url.Contains(x))) return;

            url = url.TrimStart('/');
            while (url.StartsWith("../"))
            {
                url = url.TrimStart('.', '.', '/');
            }

            if (_visited.Contains(url)) return;

            _queued.Add(new ContentUrl(url));

            _visited.Add(url);
        }

        public IEnumerable<ContentUrl> Dequeue()
        {
            _queued.Sort();
            var returnValue = _queued.ToArray();

            _queued.Clear();

            return returnValue;
        }

        public bool Any()
        {
            return _queued.Any();
        }
    }
}