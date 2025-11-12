using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using Microsoft.AspNetCore.Builder;
using NewLifeHRT.Infrastructure.Middlewares;
using NewLifeHRT.Infrastructure.Extensions;
using Finbuckle.MultiTenant.Strategies;


namespace NewLifeHRT.Infrastructure.StartupSection
{
    public static class MultiTenantExtensions
    {
        public static IServiceCollection RegisterMultiTenancyWithPathStrategy(this IServiceCollection services, IConfiguration configuration)
        {
            var multiTenancySettings = configuration.GetSection(AppSettingKeys.MultiTenancySettings).Get<MultiTenancySettings>();
            var routeParamName = multiTenancySettings.RouteTemplate?.TrimStart('{').TrimEnd('}') ?? "tenant";  // Extract "tenant" from "{tenant}"
            Console.WriteLine($"Configuring MultiTenancy with RouteParam: {routeParamName}");  // Updated log

            services.AddMultiTenant<MultiTenantInfo>()
                .WithRouteStrategy(routeParamName)  // Pass "tenant", not "{tenant}"
                .WithStore<MultiTenantInfoStore>(ServiceLifetime.Singleton);
                //.WithPerTenantAuthentication();

            services.AddScoped<UnknownTenantMiddleware>();
            return services;
        }
        public static IServiceCollection RegisterMultiTenancyWithDelegateStrategy(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            services.Configure<MultiTenancySettings>(configuration.GetSection(AppSettingKeys.MultiTenancySettings));

            services.AddMultiTenant<MultiTenantInfo>()
                .WithStore<MultiTenantInfoStore>(ServiceLifetime.Singleton)
                .WithDelegateStrategy(context =>
                {
                    // Pull the current tenant from AsyncLocal during job execution
                    return Task.FromResult(TenantContextHolder.CurrentTenant);
                });
            return services;
        }
        public static IApplicationBuilder SetupMultiTenancy(this IApplicationBuilder app)
        {
            return app
                .UseMultiTenant()
                .UseMiddleware<UnknownTenantMiddleware>()
                .UseMiddleware<MultiTenantLoggingMiddleware>();
        }
    }
}
