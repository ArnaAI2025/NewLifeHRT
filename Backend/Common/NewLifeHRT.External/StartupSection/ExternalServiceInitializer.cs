using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.External.Clients;
using NewLifeHRT.External.Factory;
using NewLifeHRT.External.Factory.Provider;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.External.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.StartupSection
{
    public static class ExternalServiceInitializer
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<LifeFileIntegrationProvider>();
            services.AddScoped<WellsIntegrationProvider>();
            services.AddScoped<EmpowerIntegrationProvider>();

            services.AddScoped<IIntegrationProviderFactory, IntegrationProviderFactory>();
            services.AddHttpClient<LifeFileApiClient>();
            services.AddHttpClient<EmpowerApiClient>();
            services.AddHttpClient<WellsApiClient>();
            return services;
        }
    }
}
