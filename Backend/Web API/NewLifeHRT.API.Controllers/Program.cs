using Finbuckle.MultiTenant;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewLifeHRT.Application.Services.StartupSection;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.External.StartupSection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Infrastructure.StartupSection;
using Serilog;
using System.Text;
using System.Text.Json;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using NewLifeHRT.Common.StartupSection;
using NewLifeHRT.Infrastructure.Middlewares;
using NewLifeHRT.Application.Services.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureSerilog(builder.Environment);
builder.Host.UseSerilog();

// ---------- AppSettings ----------
builder.Services.RegisterAppSettings(builder.Configuration);
Log.Information("========== Application Startup Beginning ==========");

builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection(AppSettingKeys.SecuritySettings));
Log.Information("Security settings configured.");

var appInsightsSettings = builder.Configuration.GetSection(AppSettingKeys.ApplicationInsights).Get<ApplicationInsights>();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = appInsightsSettings?.ConnectionString;
});

Log.Information("Application Insights configured from appsettings.json.");

builder.Host.UseSerilog((ctx, services, config) =>
{
    var telemetryConfig = services.GetRequiredService<TelemetryConfiguration>();
    telemetryConfig.ConnectionString = appInsightsSettings?.ConnectionString; 

    config
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.Debug()
        .WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces);
});
Log.Information("Serilog configured with Application Insights sink.");

var corsPolicyName = "AllowAngularDev";
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
builder.Services.RegisterMultiTenancyWithPathStrategy(builder.Configuration);
builder.Services.RegisterHospitalDbContext(builder.Configuration);
builder.Services.RegisterClinicDbContext();
Log.Information("Database contexts registered.");
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ClinicDbContext>()
    .AddDefaultTokenProviders();
builder.Services.RegisterTemplateGeneration(builder.Configuration);
Log.Information("Identity configured with ClinicDbContext.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(AppSettingKeys.JWTSettings).Get<JwtSettings>();
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
                Log.Warning("[JWT DEBUG] Auth failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tenantClaim = context.Principal?.FindFirst("tenant")?.Value;
                var resolvedTenant = context.HttpContext.GetMultiTenantContext<MultiTenantInfo>()?.TenantInfo?.Identifier;
                Log.Information("[JWT DEBUG] Token validated. TenantClaim={TenantClaim}, ResolvedTenant={ResolvedTenant}",
                    tenantClaim, resolvedTenant);

                if (tenantClaim != resolvedTenant)
                    context.Fail($"Tenant mismatch: claim={tenantClaim}, resolved={resolvedTenant}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddJwtSettings(builder.Configuration);
builder.Services.AddAuthenticationSettings(builder.Configuration);
Log.Information("Authentication + Multi-tenancy configured.");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NewLifeHRT API", Version = "v1" });
});
builder.Services.AddApplicationServices();
builder.Services.AddThirdPartyServices();
builder.Services.RegisterThirdPartySettings(builder.Configuration);
builder.Services.RegisterRefillDateGenerator(builder.Configuration);

builder.Services.AddRepositories();
builder.Services.AddSignalR();
builder.Services.AddCommonServices();

var app = builder.Build();

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

app.UseMiddleware<TenantEnrichmentMiddleware>();
Log.Information("Tenant enrichment middleware registered.");

app.UseHttpsRedirection();
app.UseEncryption();
Log.Information("Encryption middleware registered.");

app.UseGlobalExceptionHandling();
Log.Information("Global exception handling middleware registered.");

app.UseAuthentication();
app.UseSerilogRequestLogging();
app.UseAuthorization();

Log.Information("Auth & Serilog request logging middleware registered.");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        options.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.MapHub<SmsHub>("/smshub");

Log.Information("========== Application Startup Completed. Running app ==========");
//app.Run();
await app.RunAsync();
