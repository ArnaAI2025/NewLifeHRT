using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class AppointmentModeMappings
    {
        public static AppointmentModeResponseDto ToAppointmentModeResponseDto(this AppointmentMode mode)
        {
            return new AppointmentModeResponseDto
            {
                Id = mode.Id,
                ModeName = mode.ModeName
            };
        }

        public static List<AppointmentModeResponseDto> ToAppointmentModeResponseDtoList(this IEnumerable<AppointmentMode> modes)
        {
            return modes.Select(m => m.ToAppointmentModeResponseDto()).ToList();
        }
    }
}
