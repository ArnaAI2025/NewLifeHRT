using NewLifeHRT.Jobs.Scheduler.Interfaces;
using NewLifeHRT.Infrastructure.Interfaces;
using NewLifeHRT.Infrastructure.Models.RefillCalculation;

namespace NewLifeHRT.Jobs.Scheduler.Services
{
    public class AIService : IAIService
    {
        private readonly IRefillDateCalculator _calculator;

        public AIService(IRefillDateCalculator calculator)
        {
            _calculator = calculator;
        }
        public Task<RefillResultModel> CalculateNextRefillAsync(RefillInputModel input, CancellationToken cancellationToken = default)
             => _calculator.CalculateAsync(input, cancellationToken);
    }
}
