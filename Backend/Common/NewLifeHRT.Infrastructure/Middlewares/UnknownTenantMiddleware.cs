using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Middlewares
{
    public class UnknownTenantMiddleware : IMiddleware
    {
        private readonly ILogger<UnknownTenantMiddleware> _logger;

        public UnknownTenantMiddleware(ILogger<UnknownTenantMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            // Skip Swagger, health checks, and other non-tenant routes
            if (path != null && (
                path.StartsWith("/swagger") ||
                path.StartsWith("/favicon.ico") ||
                path.StartsWith("/index.html") ||
                path.StartsWith("/_framework") ||
                path.StartsWith("/health") ||
                path.StartsWith("/api/health")
            ))
            {
                await next(context);
                return;
            }

            //Extract tenant segment from route (e.g., /{tenant}/api/values)
            var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var tenantSegment = segments != null && segments.Length > 0 ? segments[0] : null;

            var accessor = context.RequestServices.GetRequiredService<IMultiTenantContextAccessor>();

            if (accessor.MultiTenantContext?.IsResolved != true)
            {
                _logger.LogWarning("Tenant not resolved for route segment: {TenantSegment}", tenantSegment ?? "(none)");
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Tenant not resolved for route segment: {tenantSegment ?? "(none)"}");
                await context.Response.Body.FlushAsync();
                return;
            }

            await next(context).ConfigureAwait(false);
        }
    }

}
