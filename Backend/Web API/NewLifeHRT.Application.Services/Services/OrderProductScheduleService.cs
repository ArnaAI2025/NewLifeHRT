using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class OrderProductScheduleService : IOrderProductScheduleService
    {
        private readonly IOrderProductScheduleRepository _orderProductScheduleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductScheduleSummaryRepository _orderProductScheduleSummaryRepository;
        private readonly IScheduleSummaryProcessingRepository _scheduleSummaryProcessingRepository;
        private readonly IPatientSelfReminderRepository _patientSelfReminderRepository;
        public OrderProductScheduleService(IOrderProductScheduleRepository orderProductScheduleRepository, IOrderRepository orderRepository, IOrderProductScheduleSummaryRepository orderProductScheduleSummaryRepository, IScheduleSummaryProcessingRepository scheduleSummaryProcessingRepository, IPatientSelfReminderRepository patientSelfReminderRepository)
        {
            _orderProductScheduleRepository = orderProductScheduleRepository ?? throw new ArgumentNullException(nameof(orderProductScheduleRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _orderProductScheduleSummaryRepository = orderProductScheduleSummaryRepository ?? throw new ArgumentNullException(nameof(orderProductScheduleSummaryRepository));
            _scheduleSummaryProcessingRepository = scheduleSummaryProcessingRepository ?? throw new ArgumentNullException(nameof(scheduleSummaryProcessingRepository));
            _patientSelfReminderRepository = patientSelfReminderRepository ?? throw new ArgumentNullException(nameof(patientSelfReminderRepository));
        }

        /// <summary>
        /// Retrieves all active medicine details for a given patient.
        /// </summary>
        /// <remarks>
        /// <para><b>Calculation:</b> Filters details whose EndDate is either null (ongoing) or not yet reached (>= today).</para>
        /// <para><b>Helper Logic:</b> Groups the <c>OrderProductSchedules</c> by <c>Sequence</c> to get distinct time entries,
        /// then selects the first occurrence time per group for display formatting.</para>
        /// <para><b>Reasoning for formatting:</b> The use of <c>"hh:mm tt"</c> ensures consistent 12-hour format with AM/PM indicators for user readability.</para>
        /// </remarks>
        public async Task<List<OrderProductScheduleSummaryResponseDto>> GetScheduleSummaryForPatientAsync(Guid patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var summaries = await _orderProductScheduleSummaryRepository.Query()
                .AsNoTracking()
                .Where(s => s.OrderDetail.Order.PatientId == patientId && (s.EndDate == null || s.EndDate >= today))
                .Include(s => s.OrderDetail)
                    .ThenInclude(od => od.Order)
                .Include(s => s.OrderDetail)
                    .ThenInclude(od => od.Product)
                .Include(s => s.OrderProductSchedules)
                .ToListAsync();

            var response = summaries.Select(s => new OrderProductScheduleSummaryResponseDto
            {
                OrderProductScheduleSummaryId = s.Id,
                OrderName = s.OrderDetail?.Order?.Name ?? string.Empty,
                ProductName = s.OrderDetail?.Product?.Name ?? string.Empty,
                Protocol = s.OrderDetail?.Protocol ?? string.Empty,
                OrderFulfilled = s.OrderDetail?.Order?.OrderFulFilled,
                StartDate = s.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                EndDate = s.EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,

                // Extract and format distinct times from grouped schedule sequences
                Time = s.OrderProductSchedules != null && s.OrderProductSchedules.Any()
                        ? string.Join(", ", s.OrderProductSchedules
                                            .GroupBy(op => op.Sequence)
                                            .Select(g =>
                                            {
                                                var first = g.OrderBy(op => op.OccurrenceDateAndTime)
                                                             .FirstOrDefault();
                                                return first?.OccurrenceDateAndTime?.ToString("hh:mm tt");
                                            })
                                            .Where(t => !string.IsNullOrWhiteSpace(t)))
                        : string.Empty,
                Status = s.Status != null ? s.Status.ToString() : string.Empty,

            }).ToList();

            return response;
        }

        /// <summary>
        /// Retrieves all medicine details entries for a patient within a specific date range.
        /// </summary>
        /// <remarks>
        /// <para><b>Calculation:</b> Converts DateOnly range into full-day DateTime boundaries using 
        /// <c>TimeOnly.MinValue</c> (00:00:00) and <c>TimeOnly.MaxValue</c> (23:59:59.9999999)
        /// to ensure schedules spanning entire days are included.</para>
        /// <para><b>Separation:</b> Instead of filtering after mapping, the method first narrows down schedules
        /// at the repository query level for performance.</para>
        /// </remarks>
        public async Task<List<OrderProductScheduleResponseDto>> GetSchedulesForLoggedInPatientAsync(Guid patientId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var summaries = await _orderProductScheduleSummaryRepository.Query()
                .AsNoTracking()
                .Include(s => s.OrderDetail)
                    .ThenInclude(od => od.Order)
                .Include(s => s.OrderDetail)
                    .ThenInclude(od => od.Product)
                .Include(s => s.OrderProductSchedules)
                .Where(s => s.OrderDetail.Order.PatientId == patientId &&
                    s.OrderProductSchedules.Any(op => op.OccurrenceDateAndTime >= startDateTime &&
                                                     op.OccurrenceDateAndTime <= endDateTime))
                .ToListAsync();

            var scheduleResponses = summaries
                .SelectMany(summary => summary.OrderProductSchedules
                    .Where(op => op.OccurrenceDateAndTime >= startDateTime &&
                                 op.OccurrenceDateAndTime <= endDateTime)
                    .Select(op => new OrderProductScheduleResponseDto
                    {
                        OrderProductScheduleId = op.Id,
                        OrderName = summary.OrderDetail.Order.Name,
                        ProductName = summary.OrderDetail.Product.Name,
                        Protocol = summary.OrderDetail.Protocol,
                        TimeZone = op.TimeZone,
                        OccurrenceDateAndTime = op.OccurrenceDateAndTime
                    }))
                .ToList();

            return scheduleResponses;
        }

        public async Task<OrderProductScheduleSummaryDetailResponseDto?> GetScheduleSummaryByIdAsync(Guid scheduleSummaryId)
        {
            var includes = new[]
            {
                "OrderDetail.Order",
                "OrderDetail.Product",
                "OrderProductSchedules"
            };

            var summary = await _orderProductScheduleSummaryRepository.GetWithIncludeAsync(scheduleSummaryId, includes);

            if (summary == null)
                return null;

            return summary.ToDetailDto();
        }

        /// <summary>
        /// Updates an existing schedule summary and creates a processing entry.
        /// </summary>
        /// <remarks>
        /// <para><b>Hardcoded values reasoning:</b> The status "InProgress" is used consistently to mark a record that has been
        /// initiated for background or queued processing. This aligns with the naming convention of <c>ScheduleSummaryProcessing</c>.</para>
        /// <para><b>Separation of concerns:</b> The method both updates the summary and creates a new processing record.
        /// In future, this can be split into:
        /// <list type="number">
        ///   <item><description><c>UpdateSummaryStatusAsync()</c> — handles summary state changes</description></item>
        ///   <item><description><c>CreateProcessingEntryAsync()</c> — encapsulates creation of processing entity</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public async Task<bool> UpdateScheduleSummaryAsync(Guid id, UpdateOrderProductScheduleSummaryRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var existingSummary = await _orderProductScheduleSummaryRepository.GetByIdAsync(id);
            if (existingSummary == null)
                return false;

            existingSummary.Status = "InProgress";
            await _orderProductScheduleSummaryRepository.UpdateAsync(existingSummary);

            var processingEntry = new ScheduleSummaryProcessing
            {
                Id = Guid.NewGuid(),
                ScheduleSummaryId = id,
                StartDate = request.StartDate,
                TimesLocal = string.Join(",", (request.Times ?? Enumerable.Empty<string>()).Where(t => !string.IsNullOrWhiteSpace(t))),
                Days = string.Join(",", (request.SelectedDays ?? Enumerable.Empty<string>()).Where(d => !string.IsNullOrWhiteSpace(d))),
                Status = "InProgress" // Hardcoded for background process tracking
            };

            await _scheduleSummaryProcessingRepository.AddAsync(processingEntry);
            return true;
        }

        /// <summary>
        /// Creates a self-reminder for the patient.
        /// </summary>
        public async Task<bool> CreatePatientSelfReminderAsync(CreatePatientSelfReminderRequestDto request, Guid patientId)
        {
            if (request == null)
                return false;

            var reminder = new PatientSelfReminder
            {
                Id = Guid.NewGuid(),
                ReminderDateTime = request.ReminderDateTime,
                Description = request.Description,
                PatientId = patientId,
                IsActive = true // By default, a new reminder is active
            };

            await _patientSelfReminderRepository.AddAsync(reminder);
            return true;
        }

        public async Task<List<PatientSelfReminderResponseDto>> GetPatientSelfRemindersAsync(Guid patientId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var reminders = await _patientSelfReminderRepository.Query()
                .AsNoTracking()
                .Where(r => r.PatientId == patientId &&
                            r.ReminderDateTime >= startDateTime &&
                            r.ReminderDateTime <= endDateTime &&
                            r.IsActive)
                .OrderBy(r => r.ReminderDateTime)
                .Select(r => new PatientSelfReminderResponseDto
                {
                    ReminderDateTime = r.ReminderDateTime,
                    Description = r.Description
                })
                .ToListAsync();

            return reminders;
        }
    }
}
