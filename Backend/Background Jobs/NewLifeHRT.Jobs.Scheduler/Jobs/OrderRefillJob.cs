using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Jobs.Scheduler.Interfaces;
using NewLifeHRT.Jobs.Scheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class OrderRefillJob :  MultiTenantJobBase<OrderRefillJob>
    {
        private readonly IOrderRefillDateProcessingService _orderRefillDateProcessingService;
        public OrderRefillJob(IServiceScopeFactory scopeFactory, ILogger<OrderRefillJob> logger, IOrderRefillDateProcessingService orderRefillDateProcessingService) : base(scopeFactory, logger)
        {
            _orderRefillDateProcessingService = orderRefillDateProcessingService;
        }

        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            await _orderRefillDateProcessingService.ProcessRefillDatesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
