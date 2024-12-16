using System.Text;
using System.Text.Json;

namespace SeleniumTest.Helper
{
    internal class OpenAiHelper
    {
        private readonly HttpClient _httpClient;

        public OpenAiHelper()
        {
            _httpClient = new HttpClient();
        }

        public async Task<LLMResponse> SendRequestToLLM(string htmlSource, string request, List<string> availableFunctions)
        {
            var payload = new
            {
                HtmlSource = htmlSource,
                Request = request,
                AvailableFunctions = availableFunctions
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("https://your-llm-endpoint.com/api", content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LLMResponse>(responseBody);
        }
    }

    public class LLMResponse
    {
        public string FunctionName { get; set; }
        public List<string> Handles { get; set; }
    }
}