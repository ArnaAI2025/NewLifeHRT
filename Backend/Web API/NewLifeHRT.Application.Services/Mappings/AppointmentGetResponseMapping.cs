using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class AppointmentGetResponseMapping
    {
        public static AppointmentGetResponseDTO ToAppointmentGetResponseDto(this Appointment appointment)
        {
            return new AppointmentGetResponseDTO
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
    }
}
