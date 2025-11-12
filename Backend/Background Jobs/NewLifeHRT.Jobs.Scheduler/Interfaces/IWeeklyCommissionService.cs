using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Jobs.Scheduler.Models;

namespace NewLifeHRT.Jobs.Scheduler.Interfaces
{
    public interface IWeeklyCommissionService
    {
        Task<Pool> CreatePool();
        Task AddActiveCounselorsToPoolAsync(Pool pool);
        Task<List<ApplicationUser>> GetAllActiveCounselorsAsync();
        Task<List<Order>> GetReadyToGenerateCommission();
        Task<CommissionReportDto> GetCommissionReportByCounselor(int counselorId);
        Task<List<PoolDetail>> GetCounselorsInCurrentPoolAsync();
    }
}
