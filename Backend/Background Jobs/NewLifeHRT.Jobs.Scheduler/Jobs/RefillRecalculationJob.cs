using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLifeHRT.Jobs.Scheduler.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Jobs
{
    public class RefillRecalculationJob : MultiTenantJobBase<RefillRecalculationJob>
    {
        private readonly IOrderRefillDateProcessingService _orderRefillDateProcessingService;
        public RefillRecalculationJob(IServiceScopeFactory scopeFactory, ILogger<RefillRecalculationJob> logger, IOrderRefillDateProcessingService orderRefillDateProcessingService) : base(scopeFactory, logger)
        {
            _orderRefillDateProcessingService = orderRefillDateProcessingService;
        }
        protected override async Task ExecuteForTenantAsync(CancellationToken cancellationToken)
        {
            await _orderRefillDateProcessingService.RefillRecalculateAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
