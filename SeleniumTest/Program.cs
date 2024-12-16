namespace SeleniumTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string apiKey = SecretManager.GetOpenAiApiKey();

            Console.Write(apiKey);
            Console.ReadLine();
        }

        static void StartYoutubeAndSearchTest()
        {
            Browser browser = new Browser();

            // Navigate to a URL
            browser.NavigateToUrl("https://www.youtube.com");

            browser.ClickButton("#content > div.body.style-scope.ytd-consent-bump-v2-lightbox > div.eom-buttons.style-scope.ytd-consent-bump-v2-lightbox > div:nth-child(1) > ytd-button-renderer:nth-child(1) > yt-button-shape > button");

            Console.WriteLine("Write what you want to search");
            string searchQuery = Console.ReadLine();

            // Fill the search box and submit the search
            browser.SendKeysToInput("#center > yt-searchbox > div.ytSearchboxComponentInputBox > form > input", searchQuery);
            browser.ClickButton("#center > yt-searchbox > button");

            string htmlSource = browser.GetCleanedHtml();
            int tokenAmount = htmlSource.TokenCounter();

            Console.WriteLine(tokenAmount);

            Console.ReadLine();
            browser.Close();
        }
    }
}