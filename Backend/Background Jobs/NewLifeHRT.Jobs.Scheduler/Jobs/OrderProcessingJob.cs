using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Models.Templates;
using NewLifeHRT.Jobs.Scheduler.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class OrderProcessingJob : MultiTenantJobBase<OrderProcessingJob>
    {
        private readonly ClinicDbContext _clinicDbContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _multiTenantContextAccessor;
        private readonly IIntegrationProviderFactory _integrationProviderFactory;
        public OrderProcessingJob(IServiceScopeFactory scopeFactory, ILogger<OrderProcessingJob> logger, ClinicDbContext clinicDbContext, IOrderProcessingService orderProcessingService, IMultiTenantContextAccessor<MultiTenantInfo> multiTenantContextAccessor, IIntegrationProviderFactory integrationProviderFactory)
        : base(scopeFactory, logger)
        {
            _clinicDbContext = clinicDbContext;
            _orderProcessingService = orderProcessingService;
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _integrationProviderFactory = integrationProviderFactory;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            var pendingOrders = await _orderProcessingService.GetPendingOrdersAsync(cancellationToken);

            Console.WriteLine("PendingOrders for Tenant " + pendingOrders.Count);

            foreach (var order in pendingOrders)
            {
                // fetch config separately
                var (type, configData) = await _orderProcessingService.GetDecryptedConfigAsync(order.PharmacyId, cancellationToken);

                if (!configData.Any())
                {
                    Console.WriteLine($"No configuration found for Pharmacy {order.PharmacyId} of Order {order.Id}");
                    continue;
                }
              
                if (string.IsNullOrEmpty(type))
                {
                    Console.WriteLine($"No IntegrationType found for Pharmacy {order.PharmacyId} of Order {order.Id}");
                    continue;
                }

                var normalizedType = type.Replace(" ", string.Empty);

                try
                {
                    var provider = _integrationProviderFactory.GetIntegrationProvider(normalizedType);

                    // send order with decrypted dictionary
                    await provider.SendOrderAsync(order.Id, configData);

                    Console.WriteLine($"Order {order.Id} sent via {normalizedType} provider.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ex " + ex);
                }
            }
        }
    }
}
