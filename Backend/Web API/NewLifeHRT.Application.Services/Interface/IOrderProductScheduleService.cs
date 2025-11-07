using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IOrderProductScheduleService
    {
        Task<List<OrderProductScheduleResponseDto>> GetSchedulesForLoggedInPatientAsync(Guid patientId, DateOnly startDate, DateOnly endDate);
        Task<List<OrderProductScheduleSummaryResponseDto>> GetScheduleSummaryForPatientAsync(Guid patientId);
        Task<OrderProductScheduleSummaryDetailResponseDto?> GetScheduleSummaryByIdAsync(Guid scheduleSummaryId);
        Task<bool> UpdateScheduleSummaryAsync(Guid id, UpdateOrderProductScheduleSummaryRequestDto request);
        Task<bool> CreatePatientSelfReminderAsync(CreatePatientSelfReminderRequestDto request, Guid patientId);
        Task<List<PatientSelfReminderResponseDto>> GetPatientSelfRemindersAsync(Guid patientId, DateOnly startDate, DateOnly endDate);
    }
}
