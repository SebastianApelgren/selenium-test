﻿using System.Text;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.VectorStores;
using OpenQA.Selenium.DevTools.V129.DeviceAccess;
using SeleniumTest.Models;

namespace SeleniumTest.Helper
{
    public class OpenAiHelper
    {
        private ChatClient client;
        private Browser browser = new Browser();

        public OpenAiHelper(string apiKey)
        {
            client = new(model: "gpt-4o-mini", apiKey:apiKey);
        }

        public async Task<List<ChatMessage>> CompleteMessagesWithToolsAsync(string? query, string? sysPrompt = null, List<ChatMessage>? messages = null)
        {
            ChatCompletionOptions options = ChatTools.Options;

            messages = CreateOrAddMessages(sysPrompt, query, messages);

            bool requiresAction;

            do
            {
                requiresAction = false;
                ChatCompletion completion = await client.CompleteChatAsync(messages, options);

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            messages.Add(new AssistantChatMessage(completion));
                            break;
                        }

                    case ChatFinishReason.ToolCalls:
                        {
                            // First, add the assistant message with tool calls to the conversation history.
                            messages.Add(new AssistantChatMessage(completion.ToolCalls));
                            // Then, add a new tool message for each tool call that is resolved.

                            foreach (ChatToolCall toolCall in completion.ToolCalls)
                            {
                                string toolResult = await CallTools(toolCall);

                                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                            }

                            requiresAction = true;
                            break;
                        }
                    case ChatFinishReason.Length:
                        throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException("Omitted content due to a content filter flag.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Deprecated in favor of tool calls.");

                    default:
                        throw new NotImplementedException(completion.FinishReason.ToString());
                }
            } while (requiresAction);

            return messages;
        }

        private async Task<string> CallTools(ChatToolCall toolCall)
        {
            Console.WriteLine($"Calling tool: {toolCall.FunctionName}");
            Console.WriteLine($"Arguments: {toolCall.FunctionArguments}");

            switch (toolCall.FunctionName)
            {
                case nameof(Browser.NavigateToUrl):
                    {
                        // The arguments that the model wants to use to call the function are specified as a
                        // stringified JSON object based on the schema defined in the tool definition. Note that
                        // the model may hallucinate arguments too. Consequently, it is important to do the
                        // appropriate parsing and validation before calling the function.
                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                        string? url = argumentsJson.RootElement.GetProperty("url").GetString();
                        browser.NavigateToUrl(url);
                        break;
                    }

                case nameof(Browser.GetCssSelector):
                    {
                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                        string? description = argumentsJson.RootElement.GetProperty("description").GetString();
                        string cssSelector = await browser.GetCssSelector(description, this);

                        Console.WriteLine($"Found CSS selector: {cssSelector}");

                        return cssSelector;
                    }

                case nameof(Browser.ClickButton):
                    {
                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                        string? cssSelector = argumentsJson.RootElement.GetProperty("cssSelector").GetString();
                        string status = browser.ClickButton(cssSelector);

                        Console.WriteLine($"Click status: {status}");

                        return status;
                    }

                case nameof(Browser.SendKeysToInput):
                    {
                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                        string? cssSelector = argumentsJson.RootElement.GetProperty("cssSelector").GetString();
                        string? keys = argumentsJson.RootElement.GetProperty("keys").GetString();
                        string status = browser.SendKeysToInput(cssSelector, keys);

                        Console.WriteLine($"Send keys status: {status}");

                        return status;
                    }

                case nameof(Browser.Close):
                    {
                        browser.Close();
                        break;
                    }

                default:
                    {
                        // Handle other unexpected calls.
                        throw new NotImplementedException();
                    }
            }

            return "Done";
        }

        public async Task<List<ChatMessage>> CompleteMessagesAsync(string? query, string? sysPrompt = null, List<ChatMessage>? messages = null)
        {
            messages = CreateOrAddMessages(sysPrompt, query, messages);

            ChatCompletion completion = await client.CompleteChatAsync(messages);

            messages.Add(new AssistantChatMessage(completion));

            return messages;
        }

        private List<ChatMessage> CreateOrAddMessages(string? sysPrompt, string? query, List<ChatMessage>? messages)
        {
            if (messages == null)
            {
                List<ChatMessage> createdMessages =
                    [
                        new SystemChatMessage(sysPrompt),
                        new UserChatMessage(query)
                    ];

                return createdMessages;
            }
            else
            {
                messages.Add(new UserChatMessage(query));
            }
            
            return messages;
        }

    }
}