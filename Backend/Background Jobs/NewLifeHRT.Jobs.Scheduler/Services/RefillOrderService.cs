using NewLifeHRT.Domain.Interfaces;
using NewLifeHRT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public sealed class RefillOrderService
    {
        private readonly IRefillDateCalculator _calculator;

        public RefillOrderService(IRefillDateCalculator calculator)
        {
            _calculator = calculator;
        }

        public Task<RefillResult> CalculateNextRefillAsync(RefillInput input, CancellationToken cancellationToken = default)
            => _calculator.CalculateAsync(input, cancellationToken);
    }
}
