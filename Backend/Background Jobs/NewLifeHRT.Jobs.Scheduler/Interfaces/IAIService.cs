using NewLifeHRT.Infrastructure.Models.RefillCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Interfaces
{
    public interface IAIService
    {
        Task<RefillResultModel> CalculateNextRefillAsync(RefillInputModel input, CancellationToken cancellationToken = default);
    }
}
