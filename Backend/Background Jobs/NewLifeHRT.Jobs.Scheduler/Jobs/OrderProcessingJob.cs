using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class OrderProcessingJob : MultiTenantJobBase<OrderProcessingJob>
    {
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IIntegrationProviderFactory _integrationProviderFactory;
        private readonly ILogger<OrderProcessingJob> _logger;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _multiTenantContextAccessor;

        public OrderProcessingJob(
            IServiceScopeFactory scopeFactory,
            ILogger<OrderProcessingJob> logger,
            IOrderProcessingService orderProcessingService,
            IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor,
            IIntegrationProviderFactory integrationProviderFactory)
            : base(scopeFactory, logger)
        {
            _orderProcessingService = orderProcessingService;
            _integrationProviderFactory = integrationProviderFactory;
            _logger = logger;
            _multiTenantContextAccessor = multiTenantContextAccessor;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            var tenant = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? "Unknown";

            _logger.LogInformation("OrderProcessingJob started for {Tenant}", tenant);

            var pendingOrders = await _orderProcessingService.GetPendingOrdersAsync(cancellationToken);
            _logger.LogInformation("Found {Count} pending orders for {Tenant}", pendingOrders.Count, tenant);

            foreach (var order in pendingOrders)
            {
                try
                {
                    var (type, config) = await _orderProcessingService.GetDecryptedConfigAsync(order.PharmacyId, cancellationToken);
                    if (string.IsNullOrEmpty(type) || !config.Any())
                    {
                        _logger.LogWarning("Missing config for pharmacy {pharmacyId} for Order {OrderId} ({Tenant})", order.PharmacyId, order.Id, tenant);
                        continue;
                    }
                    var normalizedType = type.Replace(" ", string.Empty);
                    var provider = _integrationProviderFactory.GetIntegrationProvider(type.Replace(" ", ""));
                    await provider.SendOrderAsync(order.Id, config, normalizedType);

                    _logger.LogInformation($"Order {order.Id} sent via {normalizedType} provider for {tenant}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Order {OrderId} ({Tenant})", order.Id, tenant);
                }
            }

            _logger.LogInformation("OrderProcessingJob completed for {Tenant}", tenant);
        }
    }
}
