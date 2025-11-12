using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IReminderService
    {
        Task<List<ReminderResponseDto>> GetRemindersByPatientIdAsync(Guid patientId);
        Task<List<ReminderResponseDto>> GetRemindersByLeadIdAsync(Guid leadId);
        Task<List<ReminderTypeResponseDto>> GetAllReminderTypesAsync();
        Task<List<RecurrenceRuleResponseDto>> GetAllRecurrenceRulesAsync();
        Task<CommonOperationResponseDto<Guid>> CreateReminderAsync(CreateReminderRequestDto request, int userId);
        Task<List<ReminderDashboardResponseDto>> GetTodayRemindersForAllPatientsAsync(int userId);
        Task<List<ReminderDashboardResponseDto>> GetTodayRemindersForAllLeadsAsync(int userId);
        Task<CommonOperationResponseDto<Guid>> MarkReminderAsCompletedAsync(Guid reminderId);
    }
}
