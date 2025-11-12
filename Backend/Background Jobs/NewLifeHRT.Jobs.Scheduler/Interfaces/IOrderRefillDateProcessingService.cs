using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Interfaces
{
    public interface IOrderRefillDateProcessingService
    {
        Task ProcessRefillDatesAsync(CancellationToken cancellationToken);
        Task RefillRecalculateAsync(CancellationToken cancellationToken);
    }
}
