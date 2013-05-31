using FubuMVC.CodeSnippets;

namespace FubuDocs.Snippets
{
    public class SnippetRequest
    {
        public SnippetRequest()
        {
        }

        public SnippetRequest(Snippet snippet)
        {
            Bottle = snippet.BottleName;
            Name = snippet.Name;
        }

        public string Bottle { get; set; }
        public string Name { get; set; }
    }
}