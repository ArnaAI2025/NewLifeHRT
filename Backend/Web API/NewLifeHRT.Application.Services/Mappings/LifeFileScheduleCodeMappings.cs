using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class LifeFileScheduleCodeMappings
    {
        public static LifeFileScheduleCodeResponseDto ToLifeFileScheduleCodeResponseDto(this LifeFileScheduleCode scheduleCode)
        {
            return new LifeFileScheduleCodeResponseDto
            {
                Id = scheduleCode.Id,
                Name = scheduleCode.Name
            };
        }

        public static List<LifeFileScheduleCodeResponseDto> ToLifeFileScheduleCodeResponseDtoList(this IEnumerable<LifeFileScheduleCode> scheduleCode)
        {
            return scheduleCode.Select(scheduleCode => scheduleCode.ToLifeFileScheduleCodeResponseDto()).ToList();
        }
    }
}
