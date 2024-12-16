using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTest
{
    public class Browser
    {
        private IWebDriver _driver;

        public Browser()
        {
            // Initialize the ChromeDriver
            _driver = new ChromeDriver();
        }

        public void NavigateToUrl(string url)
        {
            // Navigate to the specified URL
            _driver.Navigate().GoToUrl(url);
        }

        public string GetCleanedHtml()
        {
            // Get the HTML source of the current page
            string html = _driver.PageSource;

            // Load the HTML into HtmlAgilityPack
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Remove unnecessary elements (e.g., scripts, styles)
            doc.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());

            // Return the cleaned HTML
            return doc.DocumentNode.OuterHtml;
        }

        public void ClickButton(string cssSelector)
        {
            // Find the button element by its CSS selector and click it
            IWebElement button = _driver.FindElement(By.CssSelector(cssSelector));
            button.Click();
        }

        public void SendKeysToInput(string cssSelector, string keys)
        {
            // Find the input element by its CSS selector and send keys to it
            IWebElement input = _driver.FindElement(By.CssSelector(cssSelector));
            input.SendKeys(keys);
        }

        public void Close()
        {
            // Close the browser
            _driver.Quit();
        }
    }
}