using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenAIChat.Services
{
    public class ChatService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _cfg;

        public ChatService(IHttpClientFactory httpFactory, IConfiguration cfg)
        {
            _httpFactory = httpFactory;
            _cfg = cfg;
        }

        public record ChatMessage(string role, string content);

        public async Task<string> GetChatCompletionAsync(List<ChatMessage> messages, CancellationToken ct = default)
        {
            // Prefer Azure OpenAI if configured
            var azureEndpoint   = _cfg["AZURE_OPENAI_ENDPOINT"];
            var azureKey        = _cfg["AZURE_OPENAI_API_KEY"];
            var azureDeployment = _cfg["AZURE_OPENAI_DEPLOYMENT"];
            var azureApiVersion = _cfg["AZURE_OPENAI_API_VERSION"] ?? "2024-08-01-preview";

            var client = _httpFactory.CreateClient("openai");

            if (!string.IsNullOrWhiteSpace(azureEndpoint) &&
                !string.IsNullOrWhiteSpace(azureKey) &&
                !string.IsNullOrWhiteSpace(azureDeployment))
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("api-key", azureKey);

                var body = new
                {
                    messages = messages.Select(m => new { role = m.role, content = m.content }).ToArray(),
                    temperature = 0.3
                };
                var json = JsonSerializer.Serialize(body);
                var resp = await client.PostAsync($"{azureEndpoint!.TrimEnd('/')}/openai/deployments/{azureDeployment}/chat/completions?api-version={azureApiVersion}",
                    new StringContent(json, Encoding.UTF8, "application/json"), ct);
                var payload = await resp.Content.ReadAsStringAsync(ct);
                if (!resp.IsSuccessStatusCode) throw new InvalidOperationException($"Azure OpenAI: {resp.StatusCode} {payload}");
                using var doc = JsonDocument.Parse(payload);
                return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            }
            else
            {
                var key   = _cfg["OPENAI_API_KEY"] ?? throw new InvalidOperationException("OPENAI_API_KEY is not set.");
                var model = _cfg["OPENAI_MODEL"] ?? "gpt-4o-mini";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                var body = new
                {
                    model,
                    messages = messages.Select(m => new { role = m.role, content = m.content }).ToArray(),
                    temperature = 0.3
                };
                var json = JsonSerializer.Serialize(body);
                var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"), ct);
                var payload = await resp.Content.ReadAsStringAsync(ct);
                if (!resp.IsSuccessStatusCode) throw new InvalidOperationException($"OpenAI: {resp.StatusCode} {payload}");
                using var doc = JsonDocument.Parse(payload);
                return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            }
        }
    }
}
