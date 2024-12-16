using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

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

        public string GetHtml()
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

            // Extract text content from the HTML
            var textContent = new StringBuilder();
            foreach (var node in doc.DocumentNode.SelectNodes("//text()[normalize-space()]"))
            {
                textContent.AppendLine(node.InnerText.Trim());
            }

            // Return the cleaned HTML
            return doc.DocumentNode.OuterHtml;
        }

        public string ClickButton(string cssSelector)
        {
            try
            {
                // Find the button element by its CSS selector and click it
                IWebElement button = _driver.FindElement(By.CssSelector(cssSelector));
                button.Click();
                return "Done.";
            }
            catch (NoSuchElementException ex)
            {
                return $"Button with CSS selector '{cssSelector}' not found: {ex.Message}";
            }
            catch (ElementNotInteractableException ex)
            {
                return $"Button with CSS selector '{cssSelector}' is not interactable: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An error occurred while clicking the button with CSS selector '{cssSelector}': {ex.Message}";
            }
        }

        public string SendKeysToInput(string cssSelector, string keys)
        {
            try
            {
                // Find the input element by its CSS selector and send keys to it
                IWebElement input = _driver.FindElement(By.CssSelector(cssSelector));
                input.SendKeys(keys);
                return "Done.";
            }
            catch (NoSuchElementException ex)
            {
                return $"Input element with CSS selector '{cssSelector}' not found: {ex.Message}";
            }
            catch (ElementNotInteractableException ex)
            {
                return $"Input element with CSS selector '{cssSelector}' is not interactable: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An error occurred while sending keys to the input element with CSS selector '{cssSelector}': {ex.Message}";
            }
        }

        public void Close()
        {
            // Close the browser
            _driver.Quit();
        }
    }
}