using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class LifeFileScheduleCodeService : ILifeFileScheduleCodeService
    {
        private readonly ILifeFileScheduleCodeRepository _lifeFileScheduleCodeRepository;
        public LifeFileScheduleCodeService(ILifeFileScheduleCodeRepository lifeFileScheduleCodeRepository)
        {
            _lifeFileScheduleCodeRepository = lifeFileScheduleCodeRepository;
        }

        public async Task<List<LifeFileScheduleCodeResponseDto>> GetAllLifeFileScheduleCodesAsync()
        {
            var lifeFileScheduleCodes = await _lifeFileScheduleCodeRepository.GetAllAsync();
            return lifeFileScheduleCodes.ToLifeFileScheduleCodeResponseDtoList();
        }
    }
}
