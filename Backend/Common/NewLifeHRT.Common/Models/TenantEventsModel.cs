using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Models
{
    public record TenantEventsEventArgs(IServiceProvider ServiceProvider, MultiTenantInfo TenantInfo, CancellationToken CancellationToken);
    public record TenantLoadingEventArgs(IServiceProvider ServiceProvider, CancellationToken CancellationToken)
    {
        public MultiTenantInfo TenantInfo { get; set; }
    }

    public class TenantEventsModel
    {
        public delegate Task TenantEventsHandler<T>(TenantEventsModel sender, T args);

        public event TenantEventsHandler<TenantLoadingEventArgs> OnTenantLoading;
        public event TenantEventsHandler<TenantEventsEventArgs> OnTenantLoaded;
        public event TenantEventsHandler<TenantEventsEventArgs> OnTenantStopping;

        public async Task<MultiTenantInfo> OnTenantLoadingAsync(IServiceProvider serviceProvider, MultiTenantInfo tenantInfo, CancellationToken cancellationToken)
        {
            var args = new TenantLoadingEventArgs(serviceProvider, cancellationToken) { TenantInfo = tenantInfo };
            var task = OnTenantLoading?.Invoke(this, args);
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
            return args.TenantInfo;
        }

        public async Task OnTenantLoadedAsync(IServiceProvider serviceProvider, MultiTenantInfo tenantInfo, CancellationToken cancellationToken)
        {
            var args = new TenantEventsEventArgs(serviceProvider, tenantInfo, cancellationToken);
            var task = OnTenantLoaded?.Invoke(this, args);
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }

        public async Task OnTenantStoppingAsync(IServiceProvider serviceProvider, MultiTenantInfo tenantInfo, CancellationToken cancellationToken)
        {
            var args = new TenantEventsEventArgs(serviceProvider, tenantInfo, cancellationToken);
            var task = OnTenantStopping?.Invoke(this, args);
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }
        }
    }
}
