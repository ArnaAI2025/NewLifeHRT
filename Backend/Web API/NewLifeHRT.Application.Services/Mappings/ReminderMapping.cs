using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ReminderMapping
    {
        public static ReminderResponseDto ToReminderResponseDto(this Reminder reminder)
        {
            return new ReminderResponseDto
            {
                Id = reminder.Id,
                ReminderDateTime = reminder.ReminderDateTime,
                ReminderType = reminder.ReminderType?.TypeName ?? string.Empty,
                Description = reminder.Description
            };
        }

        public static List<ReminderResponseDto> ToReminderResponseDtoList(this IEnumerable<Reminder> reminder)
        {
            return reminder.Select(reminder => reminder.ToReminderResponseDto()).ToList();
        }
    }
}
