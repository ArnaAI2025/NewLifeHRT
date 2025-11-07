using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Domain.Interfaces;
using NewLifeHRT.Domain.Models;
using NewLifeHRT.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Llm
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

        public async Task<RefillResult> CalculateAsync(RefillInput input, CancellationToken cancellationToken = default)
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
                return new RefillResult { Status = "error" };
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var json = doc.RootElement.GetProperty("candidates")[0]
                    .GetProperty("content").GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                if (string.IsNullOrWhiteSpace(json))
                    return new RefillResult { Status = "empty_response" };

                var cleaned = CleanResponse(json);
                var result = JsonSerializer.Deserialize<RefillResult>(cleaned);
                return result ?? new RefillResult { Status = "parse_failed" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing LLM response");
                return new RefillResult { Status = "exception" };
            }
        }

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
