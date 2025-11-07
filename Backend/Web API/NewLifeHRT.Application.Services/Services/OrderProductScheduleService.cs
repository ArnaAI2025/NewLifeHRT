using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            _orderProductScheduleRepository = orderProductScheduleRepository;
            _orderRepository = orderRepository;
            _orderProductScheduleSummaryRepository = orderProductScheduleSummaryRepository;
            _scheduleSummaryProcessingRepository = scheduleSummaryProcessingRepository;
            _patientSelfReminderRepository = patientSelfReminderRepository;
        }

        public async Task<List<OrderProductScheduleSummaryResponseDto>> GetScheduleSummaryForPatientAsync(Guid patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var summaries = await _orderProductScheduleSummaryRepository.Query()
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
                Time = s.OrderProductSchedules != null && s.OrderProductSchedules.Any()
                        ? string.Join(", ", s.OrderProductSchedules
                                            .GroupBy(op => op.Sequence)
                                            .Select(g =>
                                            {
                                                var first = g.OrderBy(op => op.OccurrenceDateAndTime)
                                                             .FirstOrDefault();
                                                return first?.OccurrenceDateAndTime?.ToString("hh:mm tt");
                                            })
                                            .Where(t => !string.IsNullOrEmpty(t)))
                        : string.Empty,
                Status = s.Status != null ? s.Status.ToString() : string.Empty,

            }).ToList();

            return response;
        }

        public async Task<List<OrderProductScheduleResponseDto>> GetSchedulesForLoggedInPatientAsync(Guid patientId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var summaries = await _orderProductScheduleSummaryRepository.Query()
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

        public async Task<bool> UpdateScheduleSummaryAsync(Guid id, UpdateOrderProductScheduleSummaryRequestDto request)
        {
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
                TimesLocal = string.Join(",", request.Times),
                Days = string.Join(",", request.SelectedDays),
                Status = "InProgress"
            };

            await _scheduleSummaryProcessingRepository.AddAsync(processingEntry);
            return true;
        }

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
                IsActive =  true
            };

            await _patientSelfReminderRepository.AddAsync(reminder);
            return true;
        }

        public async Task<List<PatientSelfReminderResponseDto>> GetPatientSelfRemindersAsync(Guid patientId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            var reminders = await _patientSelfReminderRepository.Query()
                .Where(r => r.PatientId == patientId &&
                            r.ReminderDateTime >= startDateTime &&
                            r.ReminderDateTime <= endDateTime &&
                            r.IsActive)
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
