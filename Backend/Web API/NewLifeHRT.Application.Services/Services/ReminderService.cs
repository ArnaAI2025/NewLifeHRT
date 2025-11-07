using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IReminderTypeRepository _reminderTypeRepository;
        private readonly IRecurrenceRuleRepository _recurrenceRuleRepository;
        private readonly ILeadReminderRepository _leadReminderRepository;
        private readonly IPatientReminderRepository _patientReminderRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IUserRepository _userRepository;
        public ReminderService(IReminderRepository reminderRepository, IReminderTypeRepository reminderTypeRepository, IRecurrenceRuleRepository recurrenceRuleRepository, ILeadReminderRepository leadReminderRepository, IPatientReminderRepository patientReminderRepository, IPatientRepository patientRepository, ILeadRepository leadRepository, IUserRepository userRepository)
        {
            _reminderRepository = reminderRepository;
            _reminderTypeRepository = reminderTypeRepository;
            _recurrenceRuleRepository = recurrenceRuleRepository;
            _leadReminderRepository = leadReminderRepository;
            _patientReminderRepository = patientReminderRepository;
            _patientRepository = patientRepository;
            _leadRepository = leadRepository;
            _userRepository = userRepository;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateReminderAsync(CreateReminderRequestDto request, int userId)
        {
            string? timezoneStandardName = null;

            if (request.PatientId.HasValue)
            {
                timezoneStandardName = await _patientRepository
                    .Query()
                    .Where(p => p.Id == request.PatientId.Value)
                    .Select(p => p.Counselor.Timezone.StandardName)
                    .FirstOrDefaultAsync();
            }
            else if (request.LeadId.HasValue)
            {
                timezoneStandardName = await _leadRepository
                    .Query()
                    .Where(l => l.Id == request.LeadId.Value)
                    .Select(l => l.Owner.Timezone.StandardName)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrEmpty(timezoneStandardName))
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = "Could not determine counselor/owner timezone."
                };
            }

            // Convert ReminderDateTime from associated counselor/owner timezone → UTC
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneStandardName);
            var reminderUtc = TimeZoneInfo.ConvertTimeToUtc(request.ReminderDateTime, timezoneInfo);

            DateTime? recurrenceEndUtc = null;
            if (request.RecurrenceEndDateTime.HasValue)
            {
                recurrenceEndUtc = TimeZoneInfo.ConvertTimeToUtc(request.RecurrenceEndDateTime.Value, timezoneInfo);
            }

            var reminder = new Reminder(
                Guid.NewGuid(),
                reminderUtc,
                request.ReminderTypeId,
                request.Description,
                request.IsRecurring,
                request.RecurrenceRuleId,
                recurrenceEndUtc,
                userId.ToString(),
                DateTime.UtcNow
            );

            await _reminderRepository.AddAsync(reminder);

            if (request.LeadId.HasValue)
            {
                var leadReminder = new LeadReminder(Guid.NewGuid(), reminder.Id, request.LeadId.Value, userId.ToString(), DateTime.UtcNow);
                await _leadReminderRepository.AddAsync(leadReminder);
            }
            else if (request.PatientId.HasValue)
            {
                var patientReminder = new PatientReminder(Guid.NewGuid(), reminder.Id, request.PatientId.Value, userId.ToString(), DateTime.UtcNow);
                await _patientReminderRepository.AddAsync(patientReminder);
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = reminder.Id,
                Message = "Reminder created successfully."
            };
        }


        public async Task<List<RecurrenceRuleResponseDto>> GetAllRecurrenceRulesAsync()
        {
            var recurrenceRules = await _recurrenceRuleRepository.GetAllAsync();
            return recurrenceRules.ToRecurrenceRuleResponseDtoList();
        }

        public async Task<List<ReminderTypeResponseDto>> GetAllReminderTypesAsync()
        {
            var reminderTypes = await _reminderTypeRepository.GetAllAsync();
            return reminderTypes.ToReminderTypeResponseDtoList();
        }

        public async Task<List<ReminderResponseDto>> GetRemindersByLeadIdAsync(Guid leadId)
        {
            var reminders = await _reminderRepository.FindWithIncludeAsync(
                new List<Expression<Func<Reminder, bool>>>
                {
                    r => r.LeadReminder != null && r.LeadReminder.LeadId == leadId
                },
                new[] { "ReminderType", "LeadReminder" },
                noTracking: true
            );

            return reminders.ToReminderResponseDtoList();
        }

        public async Task<List<ReminderResponseDto>> GetRemindersByPatientIdAsync(Guid patientId)
        {
            var reminders = await _reminderRepository.FindWithIncludeAsync(
                new List<Expression<Func<Reminder, bool>>>
                {
                    r => r.PatientReminder != null && r.PatientReminder.PatientId == patientId
                },
                new[] { "ReminderType", "PatientReminder" },
                noTracking: true
            );

            return reminders.ToReminderResponseDtoList() ;
        }

        public async Task<List<ReminderDashboardResponseDto>> GetTodayRemindersForAllLeadsAsync(int userId)
        {
            var tzName = await _userRepository.GetUserTimezoneAsync(userId);
            if (string.IsNullOrEmpty(tzName))
                throw new Exception("User timezone not configured.");

            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzName);

            //var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;

            var reminders = await _reminderRepository.FindWithIncludeAsync(
                new List<Expression<Func<Reminder, bool>>>
                {
                    r => r.LeadReminder != null && r.LeadReminder.Lead.OwnerId == userId && r.IsActive
                },
                new[] { "ReminderType", "LeadReminder", "LeadReminder.Lead" },
                noTracking: true
            );

            //var filtered = reminders
            //    .Where(r =>
            //    {
            //        var localReminderDate = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz).Date;
            //        return localReminderDate == todayLocal;
            //    });

            return reminders.ToReminderDashboardDtoListForLeads(tz);
        }

        public async Task<List<ReminderDashboardResponseDto>> GetTodayRemindersForAllPatientsAsync(int userId)
        {
            var tzName = await _userRepository.GetUserTimezoneAsync(userId);
            if (string.IsNullOrEmpty(tzName))
                throw new Exception("User timezone not configured.");

            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzName);
            //var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;

            var reminders = await _reminderRepository.FindWithIncludeAsync(
                new List<Expression<Func<Reminder, bool>>>
                {
                    r => r.PatientReminder != null &&
                    r.PatientReminder.Patient.CounselorId == userId &&
                    r.IsActive
                },
                new[] { "ReminderType", "PatientReminder", "PatientReminder.Patient" },
                noTracking: true
            );

            //var filtered = reminders
            //    .Where(r =>
            //    {
            //        var localReminderDate = TimeZoneInfo.ConvertTimeFromUtc(r.ReminderDateTime, tz).Date;
            //        return localReminderDate == todayLocal;
            //    });

            return reminders.ToReminderDashboardDtoListForPatients(tz);
        }

        public async Task<CommonOperationResponseDto<Guid>> MarkReminderAsCompletedAsync(Guid reminderId)
        {
            var reminder = await _reminderRepository.GetByIdAsync(reminderId);
            if (reminder == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = "Reminder not found."
                };
            }

            reminder.IsActive = false;
            await _reminderRepository.UpdateAsync(reminder);

            return new CommonOperationResponseDto<Guid>
            {
                Id = reminder.Id,
                Message = "Reminder marked as completed."
            };
        }
    }
}
