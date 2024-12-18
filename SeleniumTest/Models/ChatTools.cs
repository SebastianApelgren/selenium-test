using OpenAI.Chat;

namespace SeleniumTest.Models
{
    public static class ChatTools
    {
        private static readonly ChatTool navigateToUrl = ChatTool.CreateFunctionTool(
            functionName: nameof(Browser.NavigateToUrl),
            functionDescription: "Navigates to a URL",
            functionParameters: BinaryData.FromBytes("""
                {
                        "type": "object",
                        "properties": {
                                    "url": {
                                        "type": "string",
                                        "description": "The URL to navigate to"
                                    }
                                },
                        "required": ["url"]
                }
                """u8.ToArray())
        );

        private static readonly ChatTool getCssSelector = ChatTool.CreateFunctionTool(
            functionName: nameof(Browser.GetCssSelector),
            functionDescription: "Gets the CSS selector of an element on the current page",
            functionParameters: BinaryData.FromBytes("""
                {
                        "type": "object",
                        "properties": {
                                    "description": {
                                        "type": "string",
                                        "description": "Description of the element to find"
                                    }
                                },
                        "required": ["description"]
                }
                """u8.ToArray())
        );

        private static readonly ChatTool clickButton = ChatTool.CreateFunctionTool(
            functionName: nameof(Browser.ClickButton),
            functionDescription: "Clicks a button on the current page if found, otherwise lets you know",
            functionParameters: BinaryData.FromBytes("""
                {
                        "type": "object",
                        "properties": {
                                    "cssSelector": {
                                        "type": "string",
                                        "description": "The CSS selector of the button to click"
                                    }
                                },
                        "required": ["cssSelector"]
                }
                """u8.ToArray())
        );

        private static readonly ChatTool sendKeysToInput = ChatTool.CreateFunctionTool(
            functionName: nameof(Browser.SendKeysToInput),
            functionDescription: "Sends keys to an input field on the current page if found, otherwise lets you know",
            functionParameters: BinaryData.FromBytes("""
                {
                        "type": "object",
                        "properties": {
                                    "cssSelector": {
                                        "type": "string",
                                        "description": "The CSS selector of the input field"
                                    },
                                    "keys": {
                                        "type": "string",
                                        "description": "The keys to send to the input field"
                                    }
                                },
                        "required": ["cssSelector", "keys"]
                }
                """u8.ToArray())
            );

        private static readonly ChatTool close = ChatTool.CreateFunctionTool(
            functionName: nameof(Browser.Close),
            functionDescription: "Closes the browser"
        );

        public static ChatCompletionOptions Options = new()
        {
            Tools = { navigateToUrl, getCssSelector, clickButton, sendKeysToInput, close }
        };
    }
}
