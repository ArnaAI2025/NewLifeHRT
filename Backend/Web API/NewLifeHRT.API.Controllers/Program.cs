using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using NewLifeHRT.API.Controllers.Extensions;
using NewLifeHRT.Application.Services.StartupSection;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.StartupSection;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.Encryption;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.StartupSection;
using Serilog;
using System.Text.Json;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using NewLifeHRT.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using NewLifeHRT.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog ----------
builder.Configuration.ConfigureSerilog(builder.Environment);
builder.Host.UseSerilog();

// ---------- AppSettings ----------
builder.Services.RegisterAppSettings(builder.Configuration);

Log.Information("========== Application Startup Beginning ==========");

// ========= Configure Settings =========
builder.Services.Configure<SecuritySettings>(
    builder.Configuration.GetSection("SecuritySettings")
);

// ---------- CORS ----------
var corsPolicyName = "AllowAngularDev";

Log.Information("Security settings configured.");

// ========= Application Insights =========
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = "InstrumentationKey=b3847ac2-bc06-4808-901b-641ee7f3f028;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/;ApplicationId=35d792fc-372a-4b33-9534-5831285e6954";
});
Log.Information("Application Insights configured.");

// ========= Serilog sink to App Insights =========
builder.Host.UseSerilog((ctx, services, config) =>
{
    var telemetryConfig = services.GetRequiredService<TelemetryConfiguration>();
    telemetryConfig.ConnectionString = "InstrumentationKey=b3847ac2-bc06-4808-901b-641ee7f3f028;IngestionEndpoint=https://centralus-2.in.applicationinsights.azure.com/;LiveEndpoint=https://centralus.livediagnostics.monitor.azure.com/;ApplicationId=35d792fc-372a-4b33-9534-5831285e6954";

    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces);
});
Log.Information("Serilog configured with Application Insights sink.");

// ========= CORS =========
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------- Controllers ----------
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

// ---------- Multi-Tenancy ----------
// 🔹 CHANGE: Ensure placeholder matches controller route
builder.Services.RegisterMultiTenancyWithPathStrategy(builder.Configuration);

// ---------- DbContexts ----------
builder.Services.RegisterHospitalDbContext(builder.Configuration);

// ---------- Identity ----------
builder.Services.RegisterClinicDbContext();
Log.Information("Database contexts registered.");
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ClinicDbContext>()
    .AddDefaultTokenProviders();
builder.Services.RegisterTemplateGeneration(builder.Configuration);
Log.Information("Identity configured with ClinicDbContext.");

// ---------- Authentication ----------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT DEBUG] Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tenantClaim = context.Principal?.FindFirst("tenant")?.Value;
                var resolvedTenant = context.HttpContext.GetMultiTenantContext<MultiTenantInfo>()?.TenantInfo?.Identifier;
                Console.WriteLine($"[JWT DEBUG] Token validated. Tenant claim: {tenantClaim}, Resolved: {resolvedTenant}");
                if (tenantClaim != resolvedTenant)
                    context.Fail($"Tenant mismatch: claim={tenantClaim}, resolved={resolvedTenant}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddJwtSettings(builder.Configuration);
builder.Services.AddAuthenticationSettings(builder.Configuration);
Log.Information("Authentication + Multi-tenancy configured.");


// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NewLifeHRT API", Version = "v1" });
});

// ---------- Application services ----------
builder.Services.AddApplicationServices();
builder.Services.AddThirdPartyServiceInitializers();
builder.Services.RegisterThirdPartySettingsInitializers(builder.Configuration);
builder.Services.RegisterRefillDateGenerator(builder.Configuration);

builder.Services.AddRepositories();
builder.Services.AddSignalR();
builder.Services.ConfigureServices();

// ========= Build app =========
var app = builder.Build();

// ---------- FFmpeg Setup ----------
var ffmpegFolder = Path.Combine(AppContext.BaseDirectory, "ffmpeg");
if (!Directory.Exists(ffmpegFolder) || !Directory.EnumerateFileSystemEntries(ffmpegFolder).Any())
{
    Directory.CreateDirectory(ffmpegFolder);
    FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegFolder).GetAwaiter().GetResult();
}
FFmpeg.SetExecutablesPath(ffmpegFolder);
app.UseCors(corsPolicyName);
app.UseRouting();
app.SetupMultiTenancy();
// ---------- HTTPS, Encryption, Exception Handling ----------
app.UseHttpsRedirection();
app.UseEncryption();
Log.Information("Encryption middleware registered.");

app.UseGlobalExceptionHandling();
Log.Information("Global exception handling middleware registered.");


// ---------- Auth ----------
app.UseAuthentication();
Log.Information("Authentication middleware registered.");

app.UseSerilogRequestLogging();
Log.Information("Serilog request logging middleware registered.");

app.UseAuthorization();
Log.Information("Authorization middleware registered.");

// ---------- Swagger (Tenant-aware) ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        options.RoutePrefix = "swagger";
    });

}

// ---------- Controllers & Hubs ----------
app.MapControllers();
app.MapHub<SmsHub>("/smshub");

// ========= App Run =========
Log.Information("========== Application Startup Completed. Running app ==========");
app.Run();
