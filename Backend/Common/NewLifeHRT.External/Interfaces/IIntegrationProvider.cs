using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Interfaces
{
    public interface IIntegrationProvider
    {
        Task SendOrderAsync(Guid orderId, Dictionary<string, string> configData);
    }
}
