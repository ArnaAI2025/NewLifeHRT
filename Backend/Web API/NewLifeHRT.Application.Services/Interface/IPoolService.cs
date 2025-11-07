using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPoolService
    {
        Task<Pool?> GetPoolInformationAsync(DateTime fromDate, DateTime toDate, int counselorId);

    }
}
