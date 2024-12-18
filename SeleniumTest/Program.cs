using OpenAI.Chat;
using SeleniumTest.Helper;
using SeleniumTest.Models;

namespace SeleniumTest
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            OpenAiHelper openAiHelper = new OpenAiHelper(SecretManager.GetOpenAiApiKey());

            Console.WriteLine("Hello, what website do you want to go to?");
            string? query = Console.ReadLine();

            string sysPromt = Prompts.WebNavigatorPromptOneCssDescription;

            List<ChatMessage> chatMessages = await openAiHelper.CompleteMessagesWithToolsAsync(query, sysPromt);

            int counter = 0;

            while (true)
            {

                Console.WriteLine("What do you want to do next?");
                query = Console.ReadLine();

                if(query == "bye")
                {
                    break;
                }

                chatMessages = await openAiHelper.CompleteMessagesWithToolsAsync(query, messages:chatMessages);
            }
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

            Console.ReadLine();
            browser.Close();
        }
    }
}