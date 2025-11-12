using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPoolDetailService
    {
        Task<List<PoolDetailResponseDto>> GetCounselorsByDateRangeAsync(DateTime? fromDate, DateTime? toDate);
    }
}
