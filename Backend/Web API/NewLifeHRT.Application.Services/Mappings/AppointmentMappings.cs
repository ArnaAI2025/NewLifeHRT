using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class AppointmentMappings
    {
        public static AppointmentResponseDto ToAppointmentResponseDto(this Appointment appointment)
        {
            // --- Calculation: Convert appointment date + slot time into full DateTime objects ---
            var localStart = appointment.AppointmentDate.ToDateTime(appointment.Slot.StartTime);
            var localEnd = appointment.AppointmentDate.ToDateTime(appointment.Slot.EndTime);

            // --- Hardcoded default reasoning: default to "UTC" if timezone abbreviation missing ---
            // If the user's timezone abbreviation is unavailable, fall back to "UTC" to avoid null values.
            string tzAbbreviation = appointment.User?.Timezone?.Abbreviation ?? "UTC";

            // Format start and end times for display (e.g., "10:30 AM")
            string startTimeStr = localStart.ToString("h:mm tt");
            string endTimeStr = localEnd.ToString("h:mm tt");

            // ---Calculation: Build the appointment title shown on calendars or UI components ---
            // Combines Patient, Doctor, Service, Mode, and Time information into a readable string.
            string title = $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName} - " +
                   $"{appointment.Slot?.UserServiceLink?.User?.FirstName} {appointment.Slot?.UserServiceLink?.User?.LastName} - " +
                   $"{appointment.Slot?.UserServiceLink?.Service?.ServiceName} - " +
                   $"{appointment.Mode?.ModeName} - " +
                   $"{startTimeStr} to {endTimeStr} {tzAbbreviation}";

            DateTime utcStart = localStart;
            DateTime utcEnd = localEnd;

            // --- Helper logic: Convert local doctor time to UTC using TimeZoneInfo ---
            // If timezone is valid, convert the local time using the system's timezone database.
            // If the timezone cannot be found, fallback to direct UTC conversion to prevent runtime errors.
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

        public static AppointmentGetResponseDto ToAppointmentGetResponseDto(this Appointment appointment)
        {
            return new AppointmentGetResponseDto
            {
                Id = appointment.Id,
                SlotId = appointment.SlotId,
                AppointmentDate = appointment.AppointmentDate,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ModeId = appointment.ModeId,
                StatusId = appointment.StatusId,
                Description = appointment.Description,
                ServiceId = appointment.Slot.UserServiceLink.ServiceId
            };
        }

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
