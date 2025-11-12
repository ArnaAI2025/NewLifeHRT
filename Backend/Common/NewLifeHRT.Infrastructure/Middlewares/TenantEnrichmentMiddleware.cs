using Finbuckle.MultiTenant;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace NewLifeHRT.Infrastructure.Middlewares
{
    public class TenantEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;

        public TenantEnrichmentMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
        {
            _next = next;
            _telemetryClient = telemetryClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tenantCtx = context.GetMultiTenantContext<MultiTenantInfo>();
            var tenantIdentifier = tenantCtx?.TenantInfo?.Identifier ?? "unknown";
            var tenantName = tenantCtx?.TenantInfo?.Name ?? tenantIdentifier;

            var traceParent = context.Request.Headers["traceparent"].FirstOrDefault();
            var activity = Activity.Current ?? new Activity("IncomingRequest");

            if (!activity.IsAllDataRequested)
                activity.IsAllDataRequested = true;

            if (!string.IsNullOrEmpty(traceParent))
                activity.SetParentId(traceParent); 

            if (activity.Id == null)
                activity.Start(); 

            context.Response.Headers["traceparent"] = activity.Id!;

            string requestBody = string.Empty;
            context.Request.EnableBuffering();

            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            var statusCode = context.Response?.StatusCode;
            var duration = stopwatch.ElapsedMilliseconds;

            using (LogContext.PushProperty("TenantIdentifier", tenantIdentifier))
            using (LogContext.PushProperty("TenantName", tenantName))
            using (LogContext.PushProperty("TraceId", activity.TraceId.ToString()))
            {
                var trace = new TraceTelemetry(
                    $"[Tenant:{tenantName}] {context.Request.Method} {context.Request.Path} → {statusCode} in {duration}ms",
                    SeverityLevel.Information);
                trace.Properties["TelemetrySource"] = "Backend";
                trace.Properties["TraceId"] = activity.TraceId.ToString(); //unique identifier for a single trace
                trace.Properties["ParentId"] = activity.ParentId ?? string.Empty; // the immediate caller’s 
                trace.Properties["TenantIdentifier"] = tenantIdentifier;
                trace.Properties["TenantName"] = tenantName;
                trace.Properties["RequestBody"] = Truncate(requestBody, 3000);
                trace.Properties["ResponseBody"] = Truncate(responseBody, 3000);

                _telemetryClient.TrackTrace(trace);
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}
