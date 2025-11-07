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
    public class AppointmentModeService : IAppointmentModeService
    {
        private readonly IAppointmentModeRepository _appointmentModeRepository;
        public AppointmentModeService(IAppointmentModeRepository appointmentModeRepository)
        {
            _appointmentModeRepository = appointmentModeRepository;
        }
        public async Task<IEnumerable<AppointmentModeResponseDto>> GetAllAppointmentModesAsync()
        {
            var modes = await _appointmentModeRepository.GetAllAsync();
            return modes.ToAppointmentModeResponseDtoList();
        }
    }
}
