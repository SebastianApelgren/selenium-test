using HtmlAgilityPack;
using OpenAI.Chat;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTest.Models;
using SeleniumTest.Helper;
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

        public void NavigateToUrl(string? url)
        {
            // Navigate to the specified URL
            _driver.Navigate().GoToUrl(url);
        }

        private string GetHtml()
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

        public async Task<string> GetCssSelector(string? description, OpenAiHelper openAiHelper)
        {
            string html = GetHtml();

            string sysPrompt = string.Format(Prompts.FindSelectorPrompt, description);

            for (int i = 0; i < 2; i++)
            {
                int startIndex = i * (html.Length / 2 - 30);
                int length = Math.Min(html.Length - startIndex, html.Length / 2 + 30);
                string halfHtml = html.Substring(startIndex, length);

                List<ChatMessage> messages = await openAiHelper.CompleteMessagesAsync(halfHtml, sysPrompt);

                string cssSelector = messages.Last().Content[0].Text;

                if (cssSelector != "Not found")
                {
                    return cssSelector;
                }
            }

            return "Not found";
        }

        public string ClickButton(string? cssSelector)
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

        public string SendKeysToInput(string? cssSelector, string? keys)
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