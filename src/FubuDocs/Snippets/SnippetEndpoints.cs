using FubuCore;
using FubuDocs.Infrastructure;
using FubuMVC.CodeSnippets;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Urls;

namespace FubuDocs.Snippets
{
    public class SnippetEndpoints
    {
        private readonly ISnippetCache _cache;
        private readonly IUrlRegistry _urls;
        private readonly IFubuApplicationFiles _files;

        public SnippetEndpoints(ISnippetCache cache, IUrlRegistry urls, IFubuApplicationFiles files)
        {
            _cache = cache;
            _urls = urls;
            _files = files;
        }

        public Snippet get_snippet_Bottle_Name(SnippetRequest request)
        {
            return _cache.FindByBottle(request.Name, request.Bottle);
        }

        public void post_snippet_edit(SnippetRequest request)
        {
            var snippet = get_snippet_Bottle_Name(request);
            var file = _files.Find(snippet.File);

            EditorLauncher.LaunchFile(file.Path);
        }


    }

}