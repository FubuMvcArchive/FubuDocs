using OpenQA.Selenium;

namespace FubuDocsRunner.Exports
{
    public interface IPageSource
    {
        string SourceFor(DownloadToken token);
    }

    public class PageSource : IPageSource
    {
        private readonly IWebDriver _driver;

        public PageSource(IWebDriver driver)
        {
            _driver = driver;
        }

        public string SourceFor(DownloadToken token)
        {
            _driver.Navigate().GoToUrl(token.Url);
            return _driver.PageSource;
        }
    }
}