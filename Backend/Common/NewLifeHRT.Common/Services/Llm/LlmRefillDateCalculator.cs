using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Common.Settings;
using NewLifeHRT.Infrastructure.Interfaces;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;
using System.Net.Http.Json;
using System.Text.Json;

namespace NewLifeHRT.Common.Services.Llm
{
    public sealed class LlmRefillDateCalculator : IRefillDateCalculator
    {
        private readonly HttpClient _httpClient;
        private readonly AISettings _settings;
        private readonly PromptStore _promptStore;
        private readonly ILogger<LlmRefillDateCalculator> _logger;

        public LlmRefillDateCalculator(
            HttpClient httpClient,
            IOptions<AISettings> settings,
            PromptStore promptStore,
            ILogger<LlmRefillDateCalculator> logger)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _settings = settings.Value;
            _promptStore = promptStore;
            _logger = logger;
        }

        /// <summary>
        /// Calculates refill details asynchronously by sending the provided input 
        /// to an external LLM (Language Model) API, processing the response, 
        /// and returning a <see cref="RefillResultModel"/> object.
        /// </summary>
        
        public async Task<RefillResultModel> CalculateAsync(RefillInputModel input, CancellationToken cancellationToken = default)
        {
            var prompt = _promptStore.Render("refill_date.v1", input);

            var requestBody = new
            {
                contents = new[]
                {
                    new { role = "user", parts = new[] { new { text = prompt } } }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}?key={_settings.ApiKey}", requestBody, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("LLM returned {Status}: {Content}", response.StatusCode, content);
                return new RefillResultModel { Status = "error" };
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var json = doc.RootElement.GetProperty("candidates")[0]
                    .GetProperty("content").GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                if (string.IsNullOrWhiteSpace(json))
                    return new RefillResultModel { Status = "empty_response" };

                var cleaned = CleanResponse(json);
                var result = JsonSerializer.Deserialize<RefillResultModel>(cleaned);
                return result ?? new RefillResultModel { Status = "parse_failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing LLM response");
                return new RefillResultModel { Status = "exception" };
            }
        }

        /// <summary>
        /// Cleans the raw response string from the LLM by removing 
        /// Markdown-style code block markers (e.g., ```json, ```), 
        /// trimming whitespace, quotes, and other extraneous characters.
        /// </summary>
        private static string CleanResponse(string response)
        {
            var cleaned = response.Trim();
            if (cleaned.StartsWith("```json")) cleaned = cleaned[7..];
            else if (cleaned.StartsWith("```")) cleaned = cleaned[3..];
            if (cleaned.EndsWith("```")) cleaned = cleaned[..^3];
            return cleaned.Trim('`', '\n', '\r', ' ', '"');
        }
    }
}
