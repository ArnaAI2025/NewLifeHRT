using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponseDto>> GetAppointmentsAsync(DateOnly startDate, DateOnly endDate, List<int>? doctorIds);
        Task<AppointmentResultResponseDto> CreateAppointmentAsync(CreateAppointmentRequestDto request, int userId);
        Task<AppointmentResultResponseDto> UpdateAppointmentAsync(CreateAppointmentRequestDto request, Guid id, int userId);
        Task<AppointmentResultResponseDto> DeleteAppointmentAsync(Guid appointmentId);
        Task<AppointmentGetResponseDto?> GetAppointmentByIdAsync(Guid appointmentId);
        Task<List<GetAppointmentsByPatientIdResponseDto>> GetAppointmentByPatientIdAsync(Guid patientId);
    }
}
