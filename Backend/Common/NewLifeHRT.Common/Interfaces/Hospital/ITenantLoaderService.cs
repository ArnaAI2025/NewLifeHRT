using NewLifeHRT.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Interfaces.Hospital
{
    public interface ITenantLoaderService
    {
        TenantEvents Events { get; }
        Task<bool> IsTenantLoadedAsync(string tenantId, CancellationToken cancellationToken);
        Task LoadTenantsAsync(CancellationToken cancellationToken);
        Task UnloadTenantsAsync(CancellationToken cancellationToken);
        Task<bool> LoadTenantAsync(string tenantId, CancellationToken cancellationToken);
        Task<bool> UnloadTenantAsync(string tenantId, CancellationToken cancellationToken);
        Task<bool> ReloadTenantAsync(string tenantId, CancellationToken cancellationToken);
    }
}
