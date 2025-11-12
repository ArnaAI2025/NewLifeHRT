using NewLifeHRT.External.Models;
using NewLifeHRT.External.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static NewLifeHRT.External.Models.WellsAddEditPatientModel;
using System.Text.Encodings.Web;

namespace NewLifeHRT.External.Clients
{
    public class WellsApiClient
    {
        private readonly HttpClient _httpClient;
        private bool _isInitialized;
        private string _baseUrl;

        public WellsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Initialize(Dictionary<string, string> configData)
        {
            if (_isInitialized) return;

            if (!configData.TryGetValue("BaseUrl", out var baseUrl))
                throw new ArgumentException("BaseUrl is missing in configuration");

            if (!configData.TryGetValue("Key", out var apiKey))
                throw new ArgumentException("Key is missing in configuration");

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

        private async Task<string> GetResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        private async Task<string> PostInternalAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            return await GetResponseAsync(response);
        }
        public async Task<CommonResponseModel<WellsGetTokenModel.Response>> GetTokenAsync(WellsGetTokenModel.Request wellsGetTokenRequest , CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_baseUrl);

                var response = await httpClient.GetAsync($"/api/Token/Get?key={wellsGetTokenRequest.ApiKey}", cancellationToken);
                var tokenResponse = await GetResponseAsync(response);
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenResponse}");

                return new CommonResponseModel<WellsGetTokenModel.Response> { Type = ResponseTypeEnum.Success, Message="Token generated Successfully", Data = new WellsGetTokenModel.Response { Token = tokenResponse} }; 
            }
            catch (Exception ex) {
                return new CommonResponseModel<WellsGetTokenModel.Response> { Type = ResponseTypeEnum.Error, Message = ex.Message };
            }
        }
        public async Task<CommonResponseModel<WellsAddEditPatientModel.Response>> AddEditPatientAsync(
    WellsAddEditPatientModel.Request wellsAddEditPatientRequest,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var responseJson = await PostInternalAsync<string>("/api/Patient/AddEditPatient", wellsAddEditPatientRequest, cancellationToken);

                try
                {
                    var errorObj = JsonSerializer.Deserialize<WellsErrorResponse>(responseJson, _jsonOptions);

                    if (errorObj != null && errorObj.Code == 3000)
                    {
                        var allMessages = errorObj.Details != null && errorObj.Details.Any()
                            ? $"{errorObj.Description}: {string.Join("; ", errorObj.Details)}"
                            : errorObj.Description ?? "Validation errors occurred.";

                        return new CommonResponseModel<WellsAddEditPatientModel.Response>
                        {
                            Type = ResponseTypeEnum.Error,
                            Message = allMessages,
                            Data = new WellsAddEditPatientModel.Response
                            {
                                PatientID = null
                            }
                        };
                    }
                }
                catch
                {
                    
                }

                return new CommonResponseModel<WellsAddEditPatientModel.Response>
                {
                    Type = ResponseTypeEnum.Success,
                    Message = "Patient added successfully",
                    Data = new WellsAddEditPatientModel.Response
                    {
                        PatientID = responseJson
                    }
                };
            }
            catch (Exception ex)
            {
                return new CommonResponseModel<WellsAddEditPatientModel.Response>
                {
                    Type = ResponseTypeEnum.Error,
                    Message = ex.Message,
                    Data = new WellsAddEditPatientModel.Response
                    {
                        PatientID = null
                    }
                };
            }
        }

        public async Task<CommonResponseModel<WellsAddRxModel.Response>> AddRxAsync(
    WellsAddRxModel.Request wellsAddRxRequest,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var responseJson = await PostInternalAsync<string>("/api/Rx/Add", wellsAddRxRequest, cancellationToken);

                try
                {
                    var errorObj = JsonSerializer.Deserialize<WellsErrorResponse>(responseJson, _jsonOptions);

                    if (errorObj != null && errorObj.Code == 3000)
                    {
                        var allMessages = errorObj.Details != null && errorObj.Details.Any()
                            ? $"{errorObj.Description}: {string.Join("; ", errorObj.Details)}"
                            : errorObj.Description ?? "Validation errors occurred.";

                        return new CommonResponseModel<WellsAddRxModel.Response>
                        {
                            Type = ResponseTypeEnum.Error,
                            Message = allMessages,
                            Data = new WellsAddRxModel.Response
                            {
                                RxNumber = null
                            }
                        };
                    }
                }
                catch
                {
                }

                return new CommonResponseModel<WellsAddRxModel.Response>
                {
                    Type = ResponseTypeEnum.Success,
                    Message = "Rx added successfully",
                    Data = new WellsAddRxModel.Response
                    {
                        RxNumber = responseJson
                    }
                };
            }
            catch (Exception ex)
            {
                return new CommonResponseModel<WellsAddRxModel.Response>
                {
                    Type = ResponseTypeEnum.Error,
                    Message = ex.Message,
                    Data = new WellsAddRxModel.Response
                    {
                        RxNumber = null
                    }
                };
            }
        }

    }
}
