using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ReminderTypeMappings
    {
        public static ReminderTypeResponseDto ToReminderTypeResponseDto(this ReminderType entity)
        {
            return new ReminderTypeResponseDto
            {
                Id = entity.Id,
                TypeName = entity.TypeName
            };
        }

        public static List<ReminderTypeResponseDto> ToReminderTypeResponseDtoList(this IEnumerable<ReminderType> entities)
        {
            return entities.Select(e => e.ToReminderTypeResponseDto()).ToList();
        }
    }
}
