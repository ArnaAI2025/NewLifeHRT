using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Common.Interfaces.Hospital;
using NewLifeHRT.Common.Services;
using NewLifeHRT.Common.Services.Hospital;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.StartupSection
{
    public static class CommonServiceInitializer
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<ITenantLoaderService, TenantLoaderService>();
            services.AddTransient<IClinicMigrationService, ClinicMigrationService>();
            services.AddSingleton<ISingletonService, SingletonService>();
            services.AddHostedService<HostApplicationLifetimeEventsHostedService>();
            return services;
        }
    }
}
