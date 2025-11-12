using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Settings;

namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class SettingsInitializer
    {
        public static IServiceCollection RegisterAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection(AppSettingKeys.AzureBlobStorageSettings));
            services.Configure<ClinicInformationSettings>(configuration.GetSection(AppSettingKeys.ClinicInformationSettings));
            services.Configure<ApplicationInsights>(configuration.GetSection(AppSettingKeys.ApplicationInsights));
            return services;
        }
    }
}
