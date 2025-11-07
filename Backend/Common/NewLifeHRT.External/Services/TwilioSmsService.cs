using Microsoft.Extensions.Options;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Models;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NewLifeHRT.External.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly HttpClient _httpClient;

        public TwilioSmsService(IOptions<TwilioSettings> twilioSettings, HttpClient httpClient)
        {
            _twilioSettings = twilioSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<string> SendSmsAsync(string to, string message)
        {
            try
            {
                TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

                var msg = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_twilioSettings.FromPhoneNumber),
                    to: new PhoneNumber(to)
                );

                return msg.Sid;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(byte[] content, string contentType)> GetTwilioMediaAsync(string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl))
                throw new ArgumentException("mediaUrl cannot be null or empty");

            // Prepare Basic Auth header value with Twilio credentials
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_twilioSettings.AccountSid}:{_twilioSettings.AuthToken}"));

            var request = new HttpRequestMessage(HttpMethod.Get, mediaUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch media from Twilio, status code: {response.StatusCode}");

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            return (contentBytes, contentType);
        }
    }
}
