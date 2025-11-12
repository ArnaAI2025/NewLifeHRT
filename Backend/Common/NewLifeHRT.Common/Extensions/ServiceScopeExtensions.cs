using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;

namespace NewLifeHRT.Common.Extensions
{
    public static class ServiceScopeExtensions
    {
        /// <summary>
        /// Sets the tenant context for the provided scope
        /// </summary>
        /// <param name="scope">The scope being extended. <see cref="AsyncServiceScope"/></param>
        /// <param name="multiTenantContext">A resolved tenant context. <see cref="IMultiTenantContext"/>
        ///   <seealso cref="ResolveTenantAsync(AsyncServiceScope, string)"/>
        ///   <seealso cref="ResolveTenantByIdAsync(AsyncServiceScope, string)"/>
        /// </param>
        /// <returns></returns>
        public static bool SetTenant(this AsyncServiceScope scope, IMultiTenantContext multiTenantContext)
        {
            ArgumentNullException.ThrowIfNull(scope, nameof(scope));

            var tenantContextAccessor = scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            tenantContextAccessor.MultiTenantContext = multiTenantContext;

            return multiTenantContext?.IsResolved == true;
        }

        /// <summary>
        /// Resolves a tenant by it's tenant id (Guid) without HttpContext
        /// </summary>
        /// <param name="scope">The scope being extended. <see cref="AsyncServiceScope"/></param>
        /// <param name="tenantId">The tenant id (Guid) <see cref="MultiTenantInfo.Id"/></param>
        /// <returns>The resolved multitenantcontext or null</returns>
        public static async Task<IMultiTenantContext> ResolveTenantByIdAsync(this AsyncServiceScope scope, string tenantId)
        {
            var tenantStores = scope.ServiceProvider.GetRequiredService<IEnumerable<IMultiTenantStore<MultiTenantInfo>>>();
            var tenantStore = tenantStores.OfType<MultiTenantInfoStore>().FirstOrDefault();
            Debug.Assert(tenantStore != null, "No tenant store found");

            var tenantInfo = await tenantStore.TryGetAsync(tenantId).ConfigureAwait(false);
            return tenantInfo != null
                ? new MultiTenantContext<MultiTenantInfo>
                {
                    TenantInfo = tenantInfo,
                    StoreInfo = null,
                    StrategyInfo = null
                }
                : null;
        }

        /// <summary>
        /// Resolves a tenant by it's tenant identifier (Domain) without HttpContext
        /// </summary>
        /// <param name="scope">The scope being extended. <see cref="AsyncServiceScope"/></param>
        /// <param name="tenantIdentifier">The tenant identifier (Domain) <see cref="MultiTenantInfo.Identifier"/></param>
        /// <returns>The resolved multitenantcontext or null</returns>
        public static async Task<IMultiTenantContext> ResolveTenantAsync(this AsyncServiceScope scope, string tenantIdentifier)
        {
            var tenantStores = scope.ServiceProvider.GetRequiredService<IEnumerable<IMultiTenantStore<MultiTenantInfo>>>();
            var tenantStore = tenantStores.OfType<MultiTenantInfoStore>().FirstOrDefault();
            Debug.Assert(tenantStore != null, "No tenant store found");

            var tenantInfo = await tenantStore.TryGetByIdentifierAsync(tenantIdentifier).ConfigureAwait(false);
            return tenantInfo != null
                ? new MultiTenantContext<MultiTenantInfo>
                {
                    TenantInfo = tenantInfo,
                    StoreInfo = null,
                    StrategyInfo = null
                }
                : null;
        }
    }
}
