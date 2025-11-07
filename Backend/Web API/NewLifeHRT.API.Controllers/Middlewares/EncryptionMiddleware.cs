using Microsoft.Extensions.Options;
using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Infrastructure.Middlewares;
using NewLifeHRT.Infrastructure.Models.Encryption;
using System.Text;
using System.Text.Json;

namespace NewLifeHRT.API.Controllers.Middlewares
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecuritySettings _settings;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public EncryptionMiddleware(RequestDelegate next, IOptions<SecuritySettings> options, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _settings = options.Value;
            _logger = logger;

            _logger.LogInformation("EncryptionMiddleware registered.");

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (_settings.UseEncryption && !ShouldSkip(context.Request.Path))
                {
                    await DecryptRequestBody(context);

                    var originalBody = context.Response.Body;
                    using var interceptor = new ResponseInterceptorStream(originalBody);
                    context.Response.Body = interceptor;

                    await _next(context);

                    if (context.Response.ContentType?.Contains("application/json") == true)
                    {
                        var response = interceptor.GetContent();
                        var encrypted = CryptoHelper.Encrypt(response, _settings.Key, _settings.IV);
                        var wrapper = JsonSerializer.Serialize(new EncryptedResponse { Data = encrypted }, _jsonOptions);

                        var encryptedBytes = Encoding.UTF8.GetBytes(wrapper);
                        context.Response.ContentLength = encryptedBytes.Length;
                        context.Response.Body = originalBody;
                        await context.Response.Body.WriteAsync(encryptedBytes);
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                // Log this properly in production
                throw new InvalidOperationException("Encryption middleware failed", ex);
            }
        }

        private bool ShouldSkip(string path) =>
            _settings.ExcludedPaths?.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)) == true;

        private async Task DecryptRequestBody(HttpContext context)
        {
            if (context.Request.Body == null || context.Request.ContentLength == 0)
                return;

            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(requestBody))
                return;

            try
            {
                var encryptedRequest = JsonSerializer.Deserialize<EncryptedRequest>(requestBody, _jsonOptions);

                if (string.IsNullOrWhiteSpace(encryptedRequest?.Data))
                    return;

                var decryptedJson = CryptoHelper.Decrypt(encryptedRequest.Data, _settings.Key, _settings.IV);

                var decryptedBytes = Encoding.UTF8.GetBytes(decryptedJson);
                context.Request.Body = new MemoryStream(decryptedBytes);
                context.Request.ContentLength = decryptedBytes.Length;
                context.Request.ContentType = "application/json";
                context.Request.Body.Position = 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid encrypted request", ex);
            }
        }
    }
}
