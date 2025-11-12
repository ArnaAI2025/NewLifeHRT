using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using System.Collections.Concurrent;

namespace NewLifeHRT.Infrastructure.Models.MultiTenancy
{
    public class MultiTenantInfoStore : IMultiTenantStore<MultiTenantInfo>
    {
        private ConcurrentDictionary<string, MultiTenantInfo> Tenants { get; } = new(StringComparer.InvariantCultureIgnoreCase);

        public virtual Task<MultiTenantInfo> TryGetAsync(string id)
        {
            var result = Tenants.Values.FirstOrDefault(ti => string.Equals(ti.Id, id, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(result);
        }

        public virtual Task<MultiTenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            Console.WriteLine($"Resolving tenant: {identifier}");
            Tenants.TryGetValue(identifier, out var result);
            if (result == null)
            {
                Console.WriteLine($"Tenant {identifier} not found.");
            }
            else
            {
                Console.WriteLine($"Tenant {identifier} found: Id={result.Id}, ConnectionString={result.ConnectionString}");
            }
            return Task.FromResult(result);
        }

        public virtual Task<IEnumerable<MultiTenantInfo>> GetAllAsync()
        {
            return Task.FromResult(Tenants.Values.AsEnumerable());
        }
        public virtual Task<IEnumerable<MultiTenantInfo>> GetAllAsync(int take, int skip)
        {
            return Task.FromResult(Tenants.Values.AsEnumerable().Take(take).Skip(skip));
        }
        
        public virtual Task<bool> TryAddAsync(MultiTenantInfo tenantInfo)
        {
            var added = !string.IsNullOrEmpty(tenantInfo?.Identifier) && Tenants.TryAdd(tenantInfo.Identifier, tenantInfo);
            return Task.FromResult(added);
        }

        public virtual Task<bool> TryRemoveAsync(string identifier)
        {
            var removed = Tenants.TryRemove(identifier, out var _);
            return Task.FromResult(removed);
        }

        public virtual async Task<bool> TryUpdateAsync(MultiTenantInfo tenantInfo)
        {
            var existingTenantInfo = !string.IsNullOrEmpty(tenantInfo?.Id)
                ? await TryGetAsync(tenantInfo.Id)
                : null;

            return !string.IsNullOrEmpty(existingTenantInfo?.Identifier)
                && Tenants.TryUpdate(existingTenantInfo.Identifier, tenantInfo, existingTenantInfo);
        }
    }
}
