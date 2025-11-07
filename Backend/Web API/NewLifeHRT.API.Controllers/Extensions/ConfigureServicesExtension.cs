using NewLifeHRT.Common.Interfaces.Hospital;
using NewLifeHRT.Common.Services.Hospital;
using NewLifeHRT.Common.StartupSection;

namespace NewLifeHRT.API.Controllers.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.RegisterCommonServices();
            return services;
        }
    }
}
