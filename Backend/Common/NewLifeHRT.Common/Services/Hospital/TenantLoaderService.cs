using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewLifeHRT.Common.Extensions;
using NewLifeHRT.Common.Interfaces.Hospital;
using NewLifeHRT.Common.Models;
using NewLifeHRT.Domain.Entities.Hospital;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Extensions;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;

namespace NewLifeHRT.Common.Services.Hospital
{
    public class TenantLoaderService : ITenantLoaderService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IOptions<MultiTenancySettings> multiTenancySettingsOptions;
        private readonly IEnumerable<IMultiTenantStore<MultiTenantInfo>> tenantStores;
        private readonly IHostEnvironment environment;

        public TenantLoaderService(
            IServiceProvider serviceProvider,
            ILogger<TenantLoaderService> logger,
            IOptions<MultiTenancySettings> multiTenancySettingsOptions,
            IEnumerable<IMultiTenantStore<MultiTenantInfo>> tenantStores,
            IHostEnvironment environment)
        {
            this.serviceProvider = serviceProvider;
            this.multiTenancySettingsOptions = multiTenancySettingsOptions;
            this.tenantStores = tenantStores;

            Logger = logger;
            TenantStore = tenantStores.OfType<MultiTenantInfoStore>().FirstOrDefault();
            this.environment = environment;
        }

        public TenantEvents Events { get; } = new TenantEvents();

        private MultiTenantInfoStore TenantStore { get; }
        private ILogger<TenantLoaderService> Logger { get; }

        public async Task<bool> IsTenantLoadedAsync(string tenantIdentifier, CancellationToken cancellationToken)
        {
            var tenantInfo = await TenantStore.TryGetByIdentifierAsync(tenantIdentifier);
            return tenantInfo != null;
        }

        public async Task LoadTenantsAsync(CancellationToken cancellationToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var hospitalDb = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            Logger.LogInformation("Starting tenant loading...");
            var clinics = await GetQuery(hospitalDb).ToListAsync(cancellationToken).ConfigureAwait(false);
            Logger.LogInformation("Found {Count} active clinics: {Clinics}", clinics.Count, string.Join(", ", clinics.Select(c => c.Domain)));
            if (clinics.Count == 0)
            {
                Logger.LogWarning("No active clinics found in the Clinics table. Check database and migrations.");
            }

            var tenantInfos = clinics.ToMultiTenantInfo();
            foreach (var tenant in tenantInfos)
            {
                Logger.LogInformation("Processing tenant: {Identifier}, Database: {Database}", tenant.Identifier, tenant.ConnectionString);
                if (await IsTenantLoadedAsync(tenant.Identifier, cancellationToken))
                {
                    Logger.LogTenantAlreadyLoaded(tenant.Identifier);
                    continue;
                }
                await ProcessTenantLoadAsync(tenant, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            Logger.LogFinishedLoadingTenants(tenantInfos.Count);
        }

        public async Task<bool> LoadTenantAsync(string tenantId, CancellationToken cancellationToken)
        {
            return await LoadTenantAsync(tenantId, reload: false, cancellationToken).ConfigureAwait(false);
        }

        public async Task UnloadTenantsAsync(CancellationToken cancellationToken)
        {
            var tenantInfos = (await TenantStore.GetAllAsync().ConfigureAwait(false)).ToList();
            foreach (var tenant in tenantInfos)
            {
                if (!await IsTenantLoadedAsync(tenant.Identifier, cancellationToken))
                {
                    Logger.LogTenantAlreadyUnloaded(tenant.Identifier);
                    continue;
                }
                await ProcessTenantUnloadAsync(tenant, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogFinishedUnloadingTenants(tenantInfos.Count);
        }

        public async Task<bool> UnloadTenantAsync(string tenantId, CancellationToken cancellationToken)
        {
            var tenantInfo = await TenantStore.TryGetAsync(tenantId).ConfigureAwait(false);
            if (tenantInfo != null)
            {
                return await ProcessTenantUnloadAsync(tenantInfo, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogCouldNotUnloadTenant(tenantId);
            return false;
        }

        public async Task<bool> ReloadTenantAsync(string tenantId, CancellationToken cancellationToken)
        {
            return await LoadTenantAsync(tenantId, reload: true, cancellationToken).ConfigureAwait(false);
        }

        private async Task<bool> LoadTenantAsync(string tenantId, bool reload = false, CancellationToken cancellationToken = default)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var hospitalDb = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            var tenantInfo = reload ? null : await TenantStore.TryGetAsync(tenantId).ConfigureAwait(false);
            if (tenantInfo != null)
            {
                Logger.LogTenantAlreadyLoaded(tenantId);
                return true;
            }

            if (!Guid.TryParse(tenantId, out var tenantGuid))
            {
                Logger.LogUnknownTenantFromHost(tenantId);
            }

            var clinics = await GetQuery(hospitalDb)
                .Where(client => client.TenantId == tenantGuid).ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            tenantInfo = clinics.ToMultiTenantInfo().FirstOrDefault();
            if (tenantInfo == null)
            {
                Logger.LogUnknownTenantFromHost(tenantId);
                return false;
            }

            return await ProcessTenantLoadAsync(tenantInfo, reload, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask<bool> ProcessTenantLoadAsync(MultiTenantInfo tenant, bool reload = false, CancellationToken cancellationToken = default)
        {
            using var _ = Logger.BeginScope("{scopeName}, {method} {reload} - TenantIdentifier: {TenantIdentifier}",
                nameof(TenantLoaderService), nameof(ProcessTenantLoadAsync), tenant.Identifier, reload ? "reload" : "");

                tenant.ConnectionString = GetConnectionString(tenant);
            //tenant.JwtBearerAudience = multiTenancySettingsOptions.Value.HostTemplate.Replace(Multitenancy.TenantToken, tenant.SubDomain);

            Logger.LogExecutingOnTenantLoading(tenant.Name);
            tenant = await Events.OnTenantLoadingAsync(serviceProvider, tenant, cancellationToken).ConfigureAwait(false);

            var success = reload
                ? await TenantStore.TryUpdateAsync(tenant).ConfigureAwait(false)
                : await TenantStore.TryAddAsync(tenant).ConfigureAwait(false);

            if (!success)
            {
                if (reload)
                {
                    Logger.LogCouldNotUpdateTenant(tenant.Name);
                }
                else
                {
                    Logger.LogTenantLoadError(tenant.Name, "Could not load tenant.");
                }
                return false;
            }

            try
            {
                Logger.LogExecutingOnTenantLoaded(tenant.Name);
                await Events.OnTenantLoadedAsync(serviceProvider, tenant, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error executing OnTenantLoaded for {TenantName}.", tenant.Name);
            }

            Logger.LogTenantLoaded(tenant.Name);
            return true;
        }

        private async ValueTask<bool> ProcessTenantUnloadAsync(MultiTenantInfo tenant, CancellationToken cancellationToken)
        {
            using var _ = Logger.BeginScope("{scopeName}.{method}", nameof(TenantLoaderService), nameof(ProcessTenantUnloadAsync));

            try
            {
                await Events.OnTenantStoppingAsync(serviceProvider, tenant, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error executing OnTenantStopping for {TenantName}.", tenant.Name);
            }

            if (await TenantStore.TryRemoveAsync(tenant.Identifier))
            {
                Logger.LogUnloadedTenant(tenant.Name);
                return true;
            }

            Logger.LogCouldNotUnloadTenant(tenant.Name);
            return false;
        }

        private static IQueryable<Clinic> GetQuery(HospitalDbContext hospitalDb)
        {
            return hospitalDb.Clinics
                .Where(clinic => clinic.IsActive).AsNoTracking();
        }

        private static string GetConnectionString(MultiTenantInfo tenant)
        {
            if (string.IsNullOrEmpty(tenant.DatabaseName))
            {
                throw new InvalidOperationException($"DatabaseName is null for tenant {tenant.Identifier}");
            }

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = tenant.DatabaseName,
                MultipleActiveResultSets = false,
                Encrypt = false,
                IntegratedSecurity = true
            };
            var connectionString = builder.ToString();
            Console.WriteLine($"Generated connection string for tenant {tenant.Identifier}: {connectionString}");
            return connectionString;
        }
    }
}