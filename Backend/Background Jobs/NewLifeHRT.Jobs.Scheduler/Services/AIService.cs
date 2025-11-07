using NewLifeHRT.Domain.Interfaces;
using NewLifeHRT.Domain.Models;
using NewLifeHRT.Jobs.Scheduler.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class AIService : IAIService
    {
        private readonly IRefillDateCalculator _calculator;

        public AIService(IRefillDateCalculator calculator)
        {
            _calculator = calculator;
        }
        public Task<RefillResult> CalculateNextRefillAsync(RefillInput input, CancellationToken cancellationToken = default)
             => _calculator.CalculateAsync(input, cancellationToken);
    }
}
