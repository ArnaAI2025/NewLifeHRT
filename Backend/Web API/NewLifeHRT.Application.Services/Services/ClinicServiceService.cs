using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ClinicServiceService : IClinicServiceService
    {
        private readonly IClinicServiceRepository _clinicServiceRepository;

        public ClinicServiceService(IClinicServiceRepository clinicServiceRepository)
        {
            _clinicServiceRepository = clinicServiceRepository;
        }

        public async Task<IEnumerable<AppointmentServiceResponseDto>> GetAllAppointmentServicesAsync()
        {
            var services = await _clinicServiceRepository.FindWithIncludeAsync(
                new List<Expression<Func<Service, bool>>>(),
                new[] { "UserServices.User", "UserServices.User.Timezone" } 
            );

            return services.ToAppointmentServiceResponseDtoList();
            
        }

        public async Task<IEnumerable<ClinicServiceResponseDto>> GetAllServiceByTypeAsync(string serviceTypeName)
        {
            var services = await _clinicServiceRepository.FindAsync(s => s.ServiceType == serviceTypeName);

            return services.ToClinicServiceResponseDtoList();
        }
    }
}
