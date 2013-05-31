using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace FubuDocsRunner.Exports
{
    public class DownloadPlan
    {
        private readonly ConcurrentQueue<IDownloadStep> _steps = new ConcurrentQueue<IDownloadStep>();
        private readonly string _outputDirectory;
        private readonly string _baseUrl;
        private readonly IPageSource _source;

        public DownloadPlan(string outputDirectory, string baseUrl, IPageSource source)
        {
            _outputDirectory = outputDirectory;
            _baseUrl = baseUrl;
            _source = source;
        }

        public string OutputDirectory
        {
            get { return _outputDirectory; }
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
        }

        public IEnumerable<IDownloadStep> Steps { get { return _steps; } }

        public void Add(IDownloadStep step)
        {
            if (_steps.Any(x => x.Token.Equals(step.Token)))
            {
                return;
            }

            _steps.Enqueue(step);
        }

        public DownloadReport Execute()
        {
            Console.WriteLine("Exporting to " + _outputDirectory.ToFullPath());

            var context = new DownloadContext(this, _source);

            while (true)
            {
                if (!_steps.Any())
                {
                    break;
                }

                IDownloadStep step;
                if (_steps.TryDequeue(out step))
                {
                    step.Execute(context);
                    continue;
                }

                break;
            }

            return context.Report;
        }
    }
}