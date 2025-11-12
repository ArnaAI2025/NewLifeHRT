using Microsoft.AspNetCore.Builder;
using NewLifeHRT.Infrastructure.Middlewares;
using System.Text;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseEncryption(this IApplicationBuilder app)
            => app.UseMiddleware<EncryptionMiddleware>();

        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        public static IApplicationBuilder UseTenantEnrichment(this IApplicationBuilder app)
            => app.UseMiddleware<TenantEnrichmentMiddleware>();
    }
}
