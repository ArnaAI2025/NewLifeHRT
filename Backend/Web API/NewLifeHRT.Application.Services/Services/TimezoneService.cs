using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class TimezoneService : ITimezoneService
    {
        private readonly ITimezoneRepository _timezoneRepository;

        public TimezoneService(ITimezoneRepository timezoneRepository)
        {
            _timezoneRepository = timezoneRepository;
        }
        public async Task<List<TimezoneResponseDto>> GetAllAsync()
        {
            var timezones = await _timezoneRepository.GetAllAsync();
            return timezones.ToTimezoneResponseDtoList();
        }
    }
}
