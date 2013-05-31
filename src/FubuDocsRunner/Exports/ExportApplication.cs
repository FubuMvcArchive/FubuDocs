using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuDocs.Exporting;
using FubuMVC.Katana;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace FubuDocsRunner.Exports
{
    public static class ExportApplication
    {
        public static void Export(this EmbeddedFubuMvcServer server, Action<ExportExpression> configure)
        {
            var expression = new ExportExpression(server);
            configure(expression);

            using (var runtime = expression.As<IDownloadRuntimeFactory>().Create())
            {
                runtime.Execute();
            }
        }

        public class ExportExpression : IDownloadRuntimeFactory
        {
            private string _outputDir;
            private readonly EmbeddedFubuMvcServer _server;
            private readonly IList<IDownloadReportVisitor> _visitors;
            private Func<IWebDriver> _driver;

            public ExportExpression(EmbeddedFubuMvcServer server)
            {
                _server = server;
                _visitors = new List<IDownloadReportVisitor>();

                _driver = () => new FirefoxDriver();
            }

            public void OutputTo(string outputDir)
            {
                _outputDir = outputDir;
            }

            public void UseDriver(IWebDriver driver)
            {
                _driver = () => driver;
            }

            public void AddVisitor<T>() where T : IDownloadReportVisitor, new()
            {
                AddVisitor(new T());
            }

            public void AddVisitor(IDownloadReportVisitor visitor)
            {
                _visitors.Fill(visitor);
            }

            DownloadRuntime IDownloadRuntimeFactory.Create()
            {
                var baseUrl = _server.BaseAddress;
                var driver = _driver();
                var source = new PageSource(driver);

                var plan = new DownloadPlan(_outputDir, baseUrl, source);

                var model = _server.Endpoints.Get<UrlQueryEndpoint>(x => x.get_urls()).ReadAsJson<UrlList>();
                model.Urls.Distinct().Each(url =>
                {
                    var token = DownloadToken.For(baseUrl, url);
                    plan.Add(new DownloadUrl(token, source));
                });

                return new DownloadRuntime(plan, driver, _visitors);
            }
        }

        public interface IDownloadRuntimeFactory
        {
            DownloadRuntime Create();
        }

        public class DownloadRuntime : IDisposable
        {
            private readonly IEnumerable<IDownloadReportVisitor> _visitors;

            public DownloadRuntime(DownloadPlan plan, IWebDriver driver, IEnumerable<IDownloadReportVisitor> visitors)
            {
                Plan = plan;
                Driver = driver;
                _visitors = visitors;
            }

            public DownloadPlan Plan { get; private set; }
            public IWebDriver Driver { get; private set; }

            public void Execute()
            {
                var report = Plan.Execute();
                _visitors.Each(x => x.Visit(report));
            }

            public void Dispose()
            {
                Driver.Quit();
                Driver.Dispose();
            }
        }
    }
}