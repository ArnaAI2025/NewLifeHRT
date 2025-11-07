using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ClinicServiceMappings
    {
        public static ClinicServiceResponseDto ToClinicServiceResponseDto(this Service service)
        {
            return new ClinicServiceResponseDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                DisplayName = service.DisplayName
            };
        }

        public static List<ClinicServiceResponseDto> ToClinicServiceResponseDtoList(this IEnumerable<Service> services)
        {
            return services.Select(s => s.ToClinicServiceResponseDto()).ToList();
        }

        public static AppointmentServiceResponseDto ToAppointmentServiceResponseDto(this Service service)
        {
            return new AppointmentServiceResponseDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                DisplayName = service.DisplayName,
                MaxDuration = service.MaxDuration.HasValue
                    ? $"{(int)service.MaxDuration.Value.TotalMinutes} minutes"
                    : null,
                Users = service.UserServices.Select(us => new UserDto
                {
                    UserId = us.User.Id,
                    FullName = $"{us.User.FirstName} {us.User.LastName}",
                    UserServiceLinkId = us.Id,
                    TimezoneAbbreviation = us.User.Timezone != null ? us.User.Timezone.Abbreviation : null
                }).ToList()
            };
        }

        public static List<AppointmentServiceResponseDto> ToAppointmentServiceResponseDtoList(this IEnumerable<Service> services)
        {
            return services.Select(s => s.ToAppointmentServiceResponseDto()).ToList();
        }
    }
}
