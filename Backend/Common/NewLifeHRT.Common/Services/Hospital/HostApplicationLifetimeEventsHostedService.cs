using Finbuckle.MultiTenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Common.Extensions;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Common.Interfaces.Hospital;
using NewLifeHRT.Common.Models;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Services.Hospital
{
    public class HostApplicationLifetimeEventsHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime appLifetime;
        private readonly IServiceProvider serviceProvider;
        private readonly ITenantLoaderService tenantLoaderService;

        public HostApplicationLifetimeEventsHostedService(
            ILogger<HostApplicationLifetimeEventsHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IServiceProvider serviceProvider,
            ITenantLoaderService tenantLoaderService)
        {
            Logger = logger;
            this.appLifetime = appLifetime;
            this.serviceProvider = serviceProvider;
            this.tenantLoaderService = tenantLoaderService;
        }

        private ILogger<HostApplicationLifetimeEventsHostedService> Logger { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var _ = Logger.BeginScope("HostApplicationLifetimeEventsHostedService.StartAsync");
            Logger.LogDebug("HostApplicationLifetimeEventsHostedService.StartAsync has been called.");

            // Handle deployment info
            tenantLoaderService.Events.OnTenantLoaded += DeploymentInfoServiceTenantEventsHandler<HostApplicationLifetimeEventsHostedService>.OnTenantLoadedAsync;


            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            try
            {
                await tenantLoaderService.LoadTenantsAsync(cancellationToken).ConfigureAwait(false);

                Logger.LogInformation("HostApplicationLifetimeEventsHostedService.StartAsync has completed.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during start up in HostApplicationLifetimeEventsHostedService");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var _ = Logger.BeginScope("HostApplicationLifetimeEventsHostedService.StopAsync");
            Logger.LogDebug("HostApplicationLifetimeEventsHostedService.StopAsync has been called.");

            await tenantLoaderService.UnloadTenantsAsync(cancellationToken).ConfigureAwait(false);

            // Unset for deployment info
            tenantLoaderService.Events.OnTenantLoaded -= DeploymentInfoServiceTenantEventsHandler<HostApplicationLifetimeEventsHostedService>.OnTenantLoadedAsync;

            Logger.LogInformation("HostApplicationLifetimeEventsHostedService.StopAsync has completed.");
        }

        private void OnStarted()
        {
            Logger.LogDebug("HostApplicationLifetimeEventsHostedService.OnStarted has been called.");

            Logger.LogInformation("HostApplicationLifetimeEventsHostedService.OnStarted has completed.");
        }

        private void OnStopping()
        {
            Logger.LogDebug("HostApplicationLifetimeEventsHostedService.OnStopping has been called.");

            Logger.LogInformation("HostApplicationLifetimeEventsHostedService.OnStopping has completed.");
        }

        private void OnStopped()
        {
            Logger.LogDebug("HostApplicationLifetimeEventsHostedService.OnStopped has been called.");

            Logger.LogInformation("HostApplicationLifetimeEventsHostedService.OnStopped has completed.");
        }
    }

    public static class DeploymentInfoServiceTenantEventsHandler<T> where T : class
    {
        public static async Task OnTenantLoadedAsync(TenantEventsModel _, TenantEventsEventArgs args)
        {
            await using var scope = args.ServiceProvider.CreateAsyncScope();
            var resolvedTenant = await scope.ResolveTenantAsync(args.TenantInfo.Identifier).ConfigureAwait(false);
            if (!scope.SetTenant(resolvedTenant))
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<T>>();
                logger.LogUnableToSetTenantContext(args.TenantInfo.Identifier);
                throw new MultiTenantException("Unable to set tenant context");
            }

            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
            {
                var migrationService = scope.ServiceProvider.GetRequiredService<IClinicMigrationService>();
                await migrationService.SetupClinicsDatabaseAsync(resolvedTenant.TenantInfo as MultiTenantInfo);
            }

        }
    }
}
