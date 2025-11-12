using Hangfire;
using Hangfire.SqlServer;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLifeHRT.Common.StartupSection;
using NewLifeHRT.External.StartupSection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Infrastructure.StartupSection;
using NewLifeHRT.Jobs.Scheduler.Jobs;
using NewLifeHRT.Jobs.Scheduler.StartupSection;
using Serilog;
using Serilog.Events;

public class Program
{
    static void Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                var orderJob = scope.ServiceProvider.GetRequiredService<OrderProcessingJob>();
                var weeklyCommissionJob = scope.ServiceProvider.GetRequiredService<WeeklyCommissionJob>();
                var smsJob = scope.ServiceProvider.GetRequiredService<SmsSenderJob>();
                var orderRefillJob = scope.ServiceProvider.GetRequiredService<OrderRefillJob>();
                var refillRecalculationJob = scope.ServiceProvider.GetRequiredService<RefillRecalculationJob>();

                recurringJobs.AddOrUpdate("OrderProcessingJob",
                    () => orderJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);

                recurringJobs.AddOrUpdate("WeeklyCommissionJob",
                    () => weeklyCommissionJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);

                recurringJobs.AddOrUpdate("SmsSenderJob",
                    () => smsJob.RunAsync(CancellationToken.None),
                    "0 13 * * 1-5",
                    TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));

                recurringJobs.AddOrUpdate("OrderRefillJob",
                    () => orderRefillJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);

                recurringJobs.AddOrUpdate("RefillRecalculationJob",
                    () => refillRecalculationJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);
            }

            Log.Information("Starting up NewLifeHRT Background Job host...");
            host.Run();
            Log.Information("Application shut down gracefully.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The application failed to start correctly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                SerilogInitializer.ConfigureSerilog(
                    hostingContext.Configuration,
                    hostingContext.HostingEnvironment
                );
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                var aiSettings = configuration
                    .GetSection(AppSettingKeys.ApplicationInsights)
                    .Get<ApplicationInsights>();

                if (aiSettings != null && !string.IsNullOrEmpty(aiSettings.ConnectionString))
                {
                    services.AddApplicationInsightsTelemetryWorkerService(options =>
                    {
                        options.ConnectionString = aiSettings.ConnectionString;
                    });

                    services.AddSingleton(sp =>
                    {
                        var config = TelemetryConfiguration.CreateDefault();
                        config.ConnectionString = aiSettings.ConnectionString;
                        return config;
                    });

                    services.AddSingleton<TelemetryClient>();
                }

                services.RegisterHospitalDbContext(configuration);
                services.RegisterClinicDbContext();
                services.AddAuthentication().AddJwtBearer();
                services.RegisterMultiTenancyWithDelegateStrategy(configuration);

                var connectionString = configuration.GetConnectionString(AppSettingKeys.ConnectionStringKeys.HangfireConnection);
                services.AddHangfire(config =>
                    config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true
                    })
                );
                services.AddSchedularServices();
                services.AddHangfireServer();

                services.AddScoped<OrderProcessingJob>();
                services.AddScoped<SmsSenderJob>();
                services.AddScoped<WeeklyCommissionJob>();
                services.AddScoped<OrderRefillJob>();
                services.AddScoped<RefillRecalculationJob>();
                services.AddCommonServices();
                services.RegisterThirdPartySettings(configuration);
                services.AddExternalServices();
                services.RegisterTemplateGeneration(configuration);
                services.RegisterRefillDateGenerator(configuration);
                services.Configure<SecuritySettings>(configuration.GetSection(AppSettingKeys.SecuritySettings));
                services.RegisterAppSettings(configuration);
            })
            .UseSerilog((context, services, loggerConfig) =>
            {
                var telemetryConfig = services.GetService<TelemetryConfiguration>();

                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithThreadId()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 10,
                        shared: true)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Information();

                if (telemetryConfig != null && !string.IsNullOrEmpty(telemetryConfig.ConnectionString))
                {
                    loggerConfig.WriteTo.ApplicationInsights(
                        telemetryConfig,
                        TelemetryConverter.Traces);
                }
            });
}
