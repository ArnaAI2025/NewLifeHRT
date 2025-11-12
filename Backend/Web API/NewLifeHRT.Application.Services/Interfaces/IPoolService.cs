using NewLifeHRT.Domain.Entities;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPoolService
    {
        Task<Pool?> GetPoolInformationAsync(DateTime fromDate, DateTime toDate, int counselorId);

    }
}
