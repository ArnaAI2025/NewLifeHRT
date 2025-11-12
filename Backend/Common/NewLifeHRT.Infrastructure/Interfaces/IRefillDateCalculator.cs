using NewLifeHRT.Infrastructure.Models.RefillCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Interfaces
{
    public interface IRefillDateCalculator
    {
        Task<RefillResultModel> CalculateAsync(RefillInputModel input, CancellationToken cancellationToken = default);
    }
}
