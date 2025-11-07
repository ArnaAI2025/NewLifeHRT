using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Infrastructure.Extensions;
using NewLifeHRT.Infrastructure.Models.Exceptions;
using System;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _logger.LogInformation("GlobalExceptionHandlingMiddleware registered.");
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var globalEx = ex is GlobalAPIException apiEx
                    ? apiEx
                    : (ex.InnerException is GlobalAPIException innerApiEx
                        ? innerApiEx
                        : ex.ToGlobalApiException());

                if (globalEx.StatusCode >= 500)
                    _logger.LogError(ex, "Unhandled exception converted to GlobalAPIException");
                else
                    _logger.LogWarning(ex, "Handled API Exception");

                await HandleGlobalApiExceptionAsync(httpContext, globalEx);
            }
        }

        private static async Task HandleGlobalApiExceptionAsync(HttpContext context, GlobalAPIException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = new
            {
                message = ex.Message,
                statusCode = ex.StatusCode
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
