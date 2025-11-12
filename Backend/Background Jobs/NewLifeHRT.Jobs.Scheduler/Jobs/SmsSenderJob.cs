using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System.Diagnostics;

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
            var tenantName = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown";

            _logger.LogInformation("SMS job started for TenantName: {TenantName}",tenantName);

            try
            {
                var batchMessages = await _smsSenderService.GetBatchMessageAsync();

                _logger.LogInformation("Fetched {Count} SMS messages for TenantName: {TenantName}",
                    batchMessages.Count, tenantName);

                if (batchMessages.Count == 0)
                {
                    _logger.LogInformation("No SMS messages found to process for TenantName: {TenantName}",  tenantName);
                    return;
                }

                await _smsSenderService.ProcessBulkSms(batchMessages);

                _logger.LogInformation("Successfully processed {Count} SMS messages for TenantName: {TenantName}",
                    batchMessages.Count, tenantName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing SMS job for TenantName: {TenantName}", tenantName);
                throw;
            }
            finally
            {
                _logger.LogInformation("SMS job completed for TenantName: {TenantName}",  tenantName);
            }
        }
    }
}
