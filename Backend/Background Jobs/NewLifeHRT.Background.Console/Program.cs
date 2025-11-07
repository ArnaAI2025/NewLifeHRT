using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLifeHRT.Common.StartupSection;
using NewLifeHRT.External.StartupSection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Infrastructure.Models.Encryption;
using NewLifeHRT.Infrastructure.StartupSection;
using NewLifeHRT.Jobs.Scheduler.Jobs;
using NewLifeHRT.Jobs.Scheduler.Startup;
using Serilog;

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
                var job = scope.ServiceProvider.GetRequiredService<OrderProcessingJob>();
                var weeklyCommissionJob = scope.ServiceProvider.GetRequiredService<WeeklyCommissionJob>();
                var smsJob = scope.ServiceProvider.GetRequiredService<SmsSenderJob>();
                var orderRefillJob = scope.ServiceProvider.GetRequiredService<OrderRefillJob>();
                var refillRecalculationJob = scope.ServiceProvider.GetRequiredService<RefillRecalculationJob>();


                recurringJobs.AddOrUpdate(
                    "OrderProcessingJob",
                    () => orderJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);
                recurringJobs.AddOrUpdate(
                    "WeeklyCommissionJob",
                    () => weeklyCommissionJob.RunAsync(CancellationToken.None),
                    Cron.Minutely);
                    //"0 1 * * 5");


                recurringJobs.AddOrUpdate(
                    "SmsSenderJob",
                    () => smsJob.RunAsync(CancellationToken.None),
    "               0 13 * * 1-5", 
                    TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time"));


                recurringJobs.AddOrUpdate(
                    "OrderRefillJob",
                     () => orderRefillJob.RunAsync(CancellationToken.None),
                     Cron.Minutely
                    );
                recurringJobs.AddOrUpdate(
                    "RefillRecalculationJob",
                     () => refillRecalculationJob.RunAsync(CancellationToken.None),
                     Cron.Minutely
                    );

            }

            Log.Information("Starting up NewLifeHRT application with Hangfire...");
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
                services.RegisterHospitalDbContext(configuration);
                services.RegisterClinicDbContext();
                services.AddAuthentication().AddJwtBearer();
                services.RegisterMultiTenancyWithPathStrategy(configuration); 
                var connectionString = configuration.GetConnectionString("HangfireConnection");
                services.AddHangfire(config =>
                    config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true
                    })
                );
                services.SchedularServices();
                services.AddHangfireServer();
                services.AddScoped<OrderProcessingJob>();
                services.AddScoped<SmsSenderJob>();
                services.AddScoped<WeeklyCommissionJob>();
                services.AddScoped<OrderRefillJob>();
                services.AddScoped<RefillRecalculationJob>();
                services.RegisterCommonServices();
                services.RegisterThirdPartySettingsInitializers(configuration);
                services.AddExternalServices();
                services.RegisterTemplateGeneration(configuration);
                services.RegisterRefillDateGenerator(configuration);
                services.Configure<SecuritySettings>(
                configuration.GetSection("SecuritySettings")
               );
                services.RegisterAppSettings(configuration);
            })
            .UseSerilog();
}
