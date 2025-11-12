using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class SerilogInitializer
    {
        public static void ConfigureSerilog(this IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", hostingEnvironment.ApplicationName)
                .WriteTo.File(
                    path: Path.Combine("logs", $"log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TenantIdentifier}] {Message:lj}{NewLine}{Exception}"
                    )
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
