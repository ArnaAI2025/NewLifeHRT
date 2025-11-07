using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class GetAppointmentsByPatientIdMappings
    {
        public static GetAppointmentsByPatientIdResponseDto ToAppointmentGetByPatientIdResponseDto(this Appointment appointment)
        {
            var localStart = appointment.AppointmentDate.ToDateTime(appointment.Slot.StartTime);
            var localEnd = appointment.AppointmentDate.ToDateTime(appointment.Slot.EndTime);
            string tzAbbreviation = appointment.User?.Timezone?.Abbreviation ?? "UTC";

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


            return new GetAppointmentsByPatientIdResponseDto
            {
                AppointmentId = appointment.Id,
                ServiceName = appointment.Slot.UserServiceLink.Service.ServiceName,
                DoctorName = appointment.Slot?.UserServiceLink?.User != null
                    ? $"{appointment.Slot.UserServiceLink.User.FirstName} {appointment.Slot.UserServiceLink.User.LastName}"
                    : string.Empty,
                CounselorName = appointment.Patient?.Counselor != null ? $"{appointment.Patient.Counselor.FirstName} {appointment.Patient.Counselor.LastName}" : string.Empty,
                DoctorStartDateTime = $"{localStart:MM/dd/yyyy hh:mm tt} ({tzAbbreviation})",
                DoctorEndDateTime = $"{localEnd:MM/dd/yyyy hh:mm tt} ({tzAbbreviation})",
                Status = appointment.Status.StatusName,
                Description = appointment.Description,
                UtcStartDateTime = utcStart,
                UtcEndDateTime = utcEnd,
                PatientName = appointment.Patient != null
                    ? $"{appointment.Patient.FirstName} {appointment.Patient.LastName}"
                    : string.Empty
            };
        }

        public static List<GetAppointmentsByPatientIdResponseDto> ToAppointmentGetByPatientIdResponseDtoList(this IEnumerable<Appointment> appointments)
        {
            return appointments.Select(a => a.ToAppointmentGetByPatientIdResponseDto()).ToList();
        }
    }
}
