using System.IO;

namespace FubuDocsRunner.Exports
{
    public class DownloadUrl : IDownloadStep
    {
        private readonly DownloadToken _token;
        private readonly IPageSource _source;

        public DownloadUrl(DownloadToken token, IPageSource source)
        {
            _token = token;
            _source = source;
        }

        public DownloadToken Token { get { return _token; } }

        public void Execute(DownloadContext context)
        {
            var source = _source.SourceFor(_token);
            var path = _token.GetLocalPath(context.Plan.OutputDirectory);

            source = source.Replace(_token.BaseUrl, "");

            using (var writer = new StreamWriter(File.Open(path, FileMode.CreateNew)))
            {
                writer.Write(source);
            }

            context.ItemDownloaded(_token, path);
            context.QueueDownloads(_token, source);
        }

        protected bool Equals(DownloadUrl other)
        {
            return _token.Equals(other._token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DownloadUrl) obj);
        }

        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }
    }
}