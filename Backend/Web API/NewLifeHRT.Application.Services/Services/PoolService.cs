using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Services
{
    public class PoolService : IPoolService
    {
        private IPoolRepository _poolRepository;
        public PoolService(IPoolRepository poolRepository)
        {
            _poolRepository = poolRepository;
        }
        public async Task<Pool?> GetPoolInformationAsync(DateTime fromDate, DateTime toDate, int counselorId)
        {
            if (fromDate == default || toDate == default || counselorId <= 0)
                return null; 

            var pools = await _poolRepository.FindWithIncludeAsync(
                new List<Expression<Func<Pool, bool>>> {
            p => p.FromDate == fromDate && p.ToDate == toDate
                },
                new[] { "PoolDetails" }
            );

            var pool = pools
                .FirstOrDefault(p => p.PoolDetails.Any(pd => pd.CounselorId == counselorId));

            return pool;
        }


    }
}
