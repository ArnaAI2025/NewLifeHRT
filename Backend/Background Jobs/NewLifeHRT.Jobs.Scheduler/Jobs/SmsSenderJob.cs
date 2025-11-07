using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Jobs.Scheduler.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class SmsSenderJob : MultiTenantJobBase<SmsSenderJob>
    {
        private readonly ILogger<SmsSenderJob> _logger;
        private readonly ISmsSenderService _smsSenderService;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _multiTenantContextAccessor;

        public SmsSenderJob(
            IServiceScopeFactory scopeFactory,
            ILogger<SmsSenderJob> logger,
            ISmsSenderService smsSenderService,
            IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor
        ) : base(scopeFactory, logger)
        {
            _logger = logger;
            _smsSenderService = smsSenderService;
            _multiTenantContextAccessor = multiTenantContextAccessor;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SMS job for tenant {Tenant}",
                _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown");

            var batchMessages = await _smsSenderService.GetBatchMessageAsync();

            if (batchMessages.Count == 0)
            {
                _logger.LogInformation("No SMS batch messages found.");
                return;
            }

            await _smsSenderService.ProcessBulkSms(batchMessages);

            _logger.LogInformation("Completed SMS job for tenant {Tenant}",
                _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown");
        }
    }
}
