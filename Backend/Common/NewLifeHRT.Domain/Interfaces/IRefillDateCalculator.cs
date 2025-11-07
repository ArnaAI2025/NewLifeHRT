using NewLifeHRT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces
{
    public interface IRefillDateCalculator
    {
        Task<RefillResult> CalculateAsync(RefillInput input, CancellationToken cancellationToken = default);
    }
}
