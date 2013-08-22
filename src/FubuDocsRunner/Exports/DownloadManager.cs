using System;
using System.Net;
using FubuCore;

namespace FubuDocsRunner.Exports
{
    public interface IDownloader
    {
        void Download(string url, string filePath, Action<string> continuation);
    }

    public class Downloader : IDownloader
    {
        public void Download(string url, string filePath, Action<string> continuation)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, filePath);
                }
                catch (WebException exc)
                {
                    var response = exc.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // swallow it
                        return;
                    }

                    Console.WriteLine("Failed while trying to download '{0}' to {1}", url, filePath);
                    
                    throw;
                }

                continuation(new FileSystem().ReadStringFromFile(filePath));
            }
        }
    }

    public class DownloadManager
    {
        private static IDownloader _provider;

        static DownloadManager()
        {
            Live();
        }

        public static void Live()
        {
            Stub(new Downloader());
        }

        public static void Stub(IDownloader provider)
        {
            _provider = provider;
        }

        public static void Download(string url, string path, Action<string> continuation)
        {
            _provider.Download(url, path, continuation);
        }
    }
}