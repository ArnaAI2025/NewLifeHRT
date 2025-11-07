using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Middlewares
{
    public class MultiTenantLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenantLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public async Task Invoke(HttpContext httpContext)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            var tenantIdentifier = httpContext?.GetMultiTenantContext<MultiTenantInfo>()?.TenantInfo?.Identifier;
            using (LogContext.PushProperty("TenantIdentifier", tenantIdentifier))
            {
                await _next.Invoke(httpContext).ConfigureAwait(false);
            }
        }
    }
}
