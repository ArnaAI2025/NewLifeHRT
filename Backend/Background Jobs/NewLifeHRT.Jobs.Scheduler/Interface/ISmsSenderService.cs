using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Interface
{
    public interface ISmsSenderService
    {
        Task<List<BatchMessage>> GetBatchMessageAsync();
        Task ProcessBulkSms(List<BatchMessage> batchMessages);
    }
}
