using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Interfaces
{
    public interface IOrderProcessingService
    {
        Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken);
        Task<(string, Dictionary<string, string>)> GetDecryptedConfigAsync(Guid pharmacyId, CancellationToken cancellationToken);

    }
}
