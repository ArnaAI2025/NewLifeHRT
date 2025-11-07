using Finbuckle.MultiTenant;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Common.Extensions;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public abstract class MultiTenantJobBase<TJob> where TJob: class
    {
        protected readonly IServiceScopeFactory ScopeFactory;
        protected readonly ILogger<TJob> Logger;

        protected MultiTenantJobBase(IServiceScopeFactory scopeFactory, ILogger<TJob> logger)
        {
            ScopeFactory = scopeFactory;
            Logger = logger;
        }

        protected abstract Task ExecuteForTenantAsync(CancellationToken cancellationToken);

        [DisableConcurrentExecution(5)]
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using var scope = ScopeFactory.CreateScope();
            var hospitalDb = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
            var tenants = await hospitalDb.Clinics.Where(clinic => clinic.IsActive).ToListAsync(cancellationToken);

            foreach (var tenant in tenants)
            {
                var tenantScope = ScopeFactory.CreateAsyncScope();
                var resolvedTenant = await tenantScope.ResolveTenantAsync(tenant.Domain);
                if (!tenantScope.SetTenant(resolvedTenant))
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TJob>>();
                    logger.LogUnableToSetTenantContext(tenant.Domain);
                    throw new MultiTenantException("Unable to set tenant context");
                }
                await ExecuteForTenantAsync(cancellationToken);
            }
        }
    }
}
