using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ReminderMappings
    {
        public static ReminderDashboardResponseDto ToReminderDashboardDtoForPatient(this Reminder r, TimeZoneInfo tz)
        {
            string? recurrenceDate = null;
            if (r.IsRecurring && r.RecurrenceEndDateTime.HasValue)
            {
                var localEndDate = TimeZoneInfo.ConvertTimeFromUtc(r.RecurrenceEndDateTime.Value, tz);
                recurrenceDate = localEndDate.ToString("yyyy-MM-dd");
            }
            var localReminderDate = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz);
            return new ReminderDashboardResponseDto
            {
                ReminderId = r.Id,
                Name = r.PatientReminder?.Patient != null ? $"{r.PatientReminder.Patient.FirstName} {r.PatientReminder.Patient.LastName}".Trim() : string.Empty,
                ReminderDateTime = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz),
                ReminderTypeName = r.ReminderType?.TypeName ?? string.Empty,
                Description = r.Description,
                IsRecurring = r.IsRecurring,
                RecurrenceEndDate = recurrenceDate,
                PatientId = r.PatientReminder?.PatientId
            };
        }

        public static ReminderDashboardResponseDto ToReminderDashboardDtoForLead(this Reminder r, TimeZoneInfo tz)
        {
            string? recurrenceDate = null;
            if (r.IsRecurring && r.RecurrenceEndDateTime.HasValue)
            {
                var localEndDate = TimeZoneInfo.ConvertTimeFromUtc(r.RecurrenceEndDateTime.Value, tz);
                recurrenceDate = localEndDate.ToString("yyyy-MM-dd");
            }
            var localReminderDate = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz);

            return new ReminderDashboardResponseDto
            {
                ReminderId = r.Id,
                Name = r.LeadReminder?.Lead != null ? $"{r.LeadReminder.Lead.FirstName} {r.LeadReminder.Lead.LastName}".Trim() : string.Empty,
                ReminderDateTime = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz),
                ReminderTypeName = r.ReminderType?.TypeName ?? string.Empty,
                Description = r.Description,
                IsRecurring = r.IsRecurring,
                RecurrenceEndDate= recurrenceDate,
                LeadId = r.LeadReminder?.LeadId
            };
        }

        public static List<ReminderDashboardResponseDto> ToReminderDashboardDtoListForPatients(this IEnumerable<Reminder> reminders, TimeZoneInfo tz)
        {
            return reminders.Select(r => r.ToReminderDashboardDtoForPatient(tz)).ToList();
        }

        public static List<ReminderDashboardResponseDto> ToReminderDashboardDtoListForLeads(this IEnumerable<Reminder> reminders, TimeZoneInfo tz)
        {
            return reminders.Select(r => r.ToReminderDashboardDtoForLead(tz)).ToList();
        }

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
