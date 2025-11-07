using NewLifeHRT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Interface
{
    public interface IAIService
    {
        Task<RefillResult> CalculateNextRefillAsync(RefillInput input, CancellationToken cancellationToken = default);
    }
}
