using System.Collections.Generic;

namespace FubuDocsRunner.Exports
{
    public interface IAccessedCache
    {
        void Enqueue(string url);
        IEnumerable<ContentUrl> Dequeue();

        bool Any();
    }
}