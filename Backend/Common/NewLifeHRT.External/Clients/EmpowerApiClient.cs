using NewLifeHRT.External.Models;
using NewLifeHRT.External.Enums;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using NewLifeHRT.Infrastructure.Helper;

namespace NewLifeHRT.External.Clients
{
    public class EmpowerApiClient
    {
        private readonly HttpClient _httpClient;
        private bool _isInitialized;
        private string _baseUrl;

        public EmpowerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Initialize(Dictionary<string, string> configData)
        {
            if (_isInitialized) return;

            if (!configData.TryGetValue("BaseUrl", out var baseUrl))
                throw new ArgumentException("BaseUrl is missing in configuration");

            if (!configData.TryGetValue("ApiKey", out var apiKey))
                throw new ArgumentException("Api Key is missing in configuration");

            if (!configData.TryGetValue("ApiSecret", out var apiSecret))
                throw new ArgumentException("Api Secret is missing in configuration");
            _baseUrl = baseUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
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
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),new ToStringJsonConverter() }
        };
        private async Task<T?> PostInternalAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default)
        {

            var json = JsonSerializer.Serialize(body, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            var jsonUpdated = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
                return await DeserializeResponseAsync<T>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(
                   jsonUpdated,
                    null,
                    response.StatusCode);
            }
        }
        public async Task<CommonResponseModel<EmpGetTokenModel.Response>> GetTokenAsync(EmpGetTokenModel.Request empGetTokenRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Add("APIKey", empGetTokenRequest.ApiKey);
                httpClient.DefaultRequestHeaders.Add("APISecret", empGetTokenRequest.ApiSecret);

                var response = await httpClient.PostAsync("/GetToken/Post", null, cancellationToken);
                var tokenResponse = await DeserializeResponseAsync<EmpGetTokenModel.Response>(response);
                _httpClient.DefaultRequestHeaders.Add("Token", tokenResponse.Token);

                return new CommonResponseModel<EmpGetTokenModel.Response> { Type = ResponseTypeEnum.Success, Message = "Token generated Successfully", Data = tokenResponse };
            }
            catch (Exception ex)
            {
                return new CommonResponseModel<EmpGetTokenModel.Response> { Type = ResponseTypeEnum.Error, Message = ex.Message };
            }
        }

        public async Task<CommonResponseModel<EmpPostEasyRxModel.Response>> PostEasyRxAsync(EmpPostEasyRxModel.Request empPostEasyRxRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await PostInternalAsync<EmpPostEasyRxModel.Response>("/NewRx/EasyRx", empPostEasyRxRequest, cancellationToken);
                return new CommonResponseModel<EmpPostEasyRxModel.Response> { Type = ResponseTypeEnum.Success, Message = "Rx added Successfully", Data = response };

            }
            catch (Exception ex)
            {
                return new CommonResponseModel<EmpPostEasyRxModel.Response> { Type = ResponseTypeEnum.Error, Message = ex.Message };
            }
        }

    }
}
