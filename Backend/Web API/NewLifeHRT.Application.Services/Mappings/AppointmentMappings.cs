using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class AppointmentMappings
    {
        public static AppointmentResponseDto ToAppointmentResponseDto(this Appointment appointment)
        {
            var localStart = appointment.AppointmentDate.ToDateTime(appointment.Slot.StartTime);
            var localEnd = appointment.AppointmentDate.ToDateTime(appointment.Slot.EndTime);

            string tzAbbreviation = appointment.User?.Timezone?.Abbreviation ?? "UTC";

            string startTimeStr = localStart.ToString("h:mm tt");
            string endTimeStr = localEnd.ToString("h:mm tt");

            string title = $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName} - " +
                   $"{appointment.Slot?.UserServiceLink?.User?.FirstName} {appointment.Slot?.UserServiceLink?.User?.LastName} - " +
                   $"{appointment.Slot?.UserServiceLink?.Service?.ServiceName} - " +
                   $"{appointment.Mode?.ModeName} - " +
                   $"{startTimeStr} to {endTimeStr} {tzAbbreviation}";

            DateTime utcStart = localStart;
            DateTime utcEnd = localEnd;

            if (appointment.User?.Timezone != null)
            {
                try
                {
                    var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(appointment.User.Timezone.StandardName);
                    utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, tzInfo);
                    utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd, tzInfo);
                }
                catch (TimeZoneNotFoundException)
                {
                    utcStart = localStart.ToUniversalTime();
                    utcEnd = localEnd.ToUniversalTime();
                }
            }

            return new AppointmentResponseDto
            {
                AppointmentId = appointment.Id,
                SlotId = appointment.SlotId,
                StartDateTime = utcStart,
                EndDateTime = utcEnd,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient != null
                    ? $"{appointment.Patient.FirstName} {appointment.Patient.LastName}"
                    : string.Empty,
                ModeId = appointment.ModeId,
                ModeName = appointment.Mode.ModeName,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status.StatusName,
                Description = appointment.Description,
                ServiceName = appointment.Slot.UserServiceLink.Service.ServiceName,
                DoctorId = appointment.Slot.UserServiceLink.User.Id,
                DoctorName = appointment.Slot?.UserServiceLink?.User != null
                    ? $"{appointment.Slot.UserServiceLink.User.FirstName} {appointment.Slot.UserServiceLink.User.LastName}"
                    : string.Empty,
                CounselorName = appointment.Patient?.Counselor != null ? $"{appointment.Patient.Counselor.FirstName} {appointment.Patient.Counselor.LastName}" : string.Empty,
                Title = title,
                DoctorStartDateTime = localStart,
                DoctorEndDateTime = localEnd,
                ColorCode = appointment.Slot?.UserServiceLink?.User != null
                    ? appointment.Slot.UserServiceLink.User.ColorCode
                    : string.Empty,
            };
        }

        public static List<AppointmentResponseDto> ToAppointmentResponseDtoList(this IEnumerable<Appointment> appointments)
        {
            return appointments.Select(a => a.ToAppointmentResponseDto()).ToList();
        }
    }
}
