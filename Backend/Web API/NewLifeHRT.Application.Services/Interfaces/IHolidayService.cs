using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<CommonOperationResponseDto<Guid>> CreateHolidayAsync(CreateHolidayRequestDto request, int userId);
        Task<List<HolidayResponseDto>> GetAllHolidaysAsync(GetAllHolidaysRequestDto request);
    }
}
