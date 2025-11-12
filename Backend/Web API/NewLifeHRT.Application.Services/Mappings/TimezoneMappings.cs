using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class TimezoneMappings
    {
        public static TimezoneResponseDto ToTimezoneResponseDto(this Timezone timezone)
        {
            return new TimezoneResponseDto
            {
                Id = timezone.Id,
                StandardName = timezone.StandardName,
                Abbreviation = timezone.Abbreviation
            };
        }

        public static List<TimezoneResponseDto> ToTimezoneResponseDtoList(this IEnumerable<Timezone> timezones)
        {
            return timezones.Select(a => a.ToTimezoneResponseDto()).ToList();
        }
    }
}
