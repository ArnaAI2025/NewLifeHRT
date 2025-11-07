using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PoolDetailService : IPoolDetailService
    {
        private readonly IPoolDetailRepository _poolDetailRepository;

        public PoolDetailService(IPoolDetailRepository poolDetailRepository)
        {
            _poolDetailRepository = poolDetailRepository;
        }

        public async Task<List<PoolDetailResponseDto>> GetCounselorsByDateRangeAsync(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return new List<PoolDetailResponseDto>();
            }

            var from = fromDate.Value.Date;  
            var to = toDate.Value.Date.AddDays(1).AddTicks(-1); 

            var poolDetails = await _poolDetailRepository.Query()
                .Include(pd => pd.Pool)
                .Include(pd => pd.Counselor)
                .Where(pd => pd.Pool.FromDate >= from && pd.Pool.ToDate <= to)
                .ToListAsync();

            return poolDetails.ToPoolDetailResponseDtoList();
        }




    }
}
