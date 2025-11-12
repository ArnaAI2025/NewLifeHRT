using NewLifeHRT.External.Models;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewLifeHRT.External.Clients
{
    public class LifeFileApiClient
    {
        private readonly HttpClient _httpClient;
        private bool _isInitialized;

        public LifeFileApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Initialize(Dictionary<string, string> configData, string type)
        {
            if (_isInitialized) return;

            if (!configData.TryGetValue("BaseUrl", out var baseUrl))
                throw new ArgumentException("BaseUrl is missing in configuration");

            if (!configData.TryGetValue("Username", out var username))
                throw new ArgumentException("Username is missing in configuration");

            if (!configData.TryGetValue("Password", out var password))
                throw new ArgumentException("Password is missing in configuration");

            _httpClient.BaseAddress = new Uri(baseUrl);

            // Basic Auth header
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            if (type.Equals("aps", StringComparison.OrdinalIgnoreCase))
            {
                // APS headers
                _httpClient.DefaultRequestHeaders.Add("X-Vendor-ID", "11489");
                _httpClient.DefaultRequestHeaders.Add("X-Location-ID", "110294");
                _httpClient.DefaultRequestHeaders.Add("X-API-Network-ID", "1389");
            }
            else
            {
                // LifeFile (PBP) headers
                _httpClient.DefaultRequestHeaders.Add("X-Vendor-ID", "11504");
                _httpClient.DefaultRequestHeaders.Add("X-Location-ID", "110285");
                _httpClient.DefaultRequestHeaders.Add("X-API-Network-ID", "1357");
            }

            _isInitialized = true;
        }
        private async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                return result;
            }
            catch (JsonException)
            {
                throw new HttpRequestException(
                    $"Failed to deserialize response. Status: {response.StatusCode}, Response: {json}",
                    null,
                    response.StatusCode);
            }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        private async Task<T?> PostInternalAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(body, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Request to {endpoint} failed with status {(int)response.StatusCode} {response.ReasonPhrase}. " +
                        $"Response: {errorContent}"
                    );
                }

                return await DeserializeResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PostInternalAsync ERROR] {ex}");
                throw new InvalidOperationException("An error occurred during the API POST request.", ex);
            }
        }


        public async Task<OrderResponseModel?> SendOrderAsync(object orderRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await PostInternalAsync<OrderResponseModel>("/lfapi/v1/order", orderRequest, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"SendOrderAsync failed: {ex.Message}");
                throw new InvalidOperationException("An error occurred during Send Order request.", ex);
            }
        }
    }
}
