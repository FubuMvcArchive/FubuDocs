using System;
using FubuCore;
using FubuDocsRunner.Exports;

namespace FubuDocs.Tests.Exports
{
    public class DownloadScenario
    {
        private readonly StubDownloader _downloader;
        private readonly string _directory;
        private readonly FileSystem _fileSystem;

        public DownloadScenario(StubDownloader downloader)
        {
            _downloader = downloader;

            _directory = Guid.NewGuid().ToString();
            _fileSystem = new FileSystem();

            _fileSystem.CreateDirectory(_directory);
        }

        public string Directory { get { return _directory; } }

        public StubDownloader Downloader { get { return _downloader; } }

        public string ContentsFor(string url)
        {
            return _downloader.ContentsFor(url);
        }

        public void Cleanup()
        {
            _fileSystem.DeleteDirectory(_directory);
            DownloadManager.Live();
        }

        public static DownloadScenario Create(Action<ScenarioDefinition> configure)
        {
            var scenario = new ScenarioDefinition();
            configure(scenario);

            var downloader = scenario.As<IScenarioDefinition>().BuildProvider();
            DownloadManager.Stub(downloader);

            return new DownloadScenario(downloader);
        }

        public interface IScenarioDefinition
        {
            StubDownloader BuildProvider();
        }

        public class ScenarioDefinition : IScenarioDefinition
        {
            private readonly StubDownloader _provider;

            public ScenarioDefinition()
            {
                _provider = new StubDownloader();
            }

            public ScenarioDefinition Url(string url, string contents)
            {
                _provider.Add(url, contents);
                return this;
            }

            StubDownloader IScenarioDefinition.BuildProvider()
            {
                return _provider;
            }
        }
    }
}