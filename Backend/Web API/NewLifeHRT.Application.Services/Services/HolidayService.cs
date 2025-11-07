using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepo;
        private readonly IHolidayDateRepository _holidayDateRepo;
        private readonly IHolidayRecurrenceRepository _holidayRecurrenceRepo;

        public HolidayService(IHolidayRepository holidayRepo, IHolidayDateRepository holidayDateRepo, IHolidayRecurrenceRepository holidayRecurrenceRepo)
        {
            _holidayRepo = holidayRepo;
            _holidayDateRepo = holidayDateRepo;
            _holidayRecurrenceRepo = holidayRecurrenceRepo;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateHolidayAsync(CreateHolidayRequestDto request, int userId)
        {
            var holiday = new Holiday(request.UserId, request.LeaveType, request.Description, userId.ToString(), DateTime.UtcNow);

            await _holidayRepo.AddAsync(holiday);

            if (request.HolidayDates != null && request.HolidayDates.Any())
            {
                foreach (var dateDto in request.HolidayDates)
                {
                    var holidayDate = new HolidayDate(holiday.Id, dateDto.Date, dateDto.StartTime, dateDto.EndTime, userId.ToString(), DateTime.UtcNow);
                    await _holidayDateRepo.AddAsync(holidayDate);
                    holiday.HolidayDates.Add(holidayDate);
                }
            }

            if (request.Recurrence != null)
            {
                var recurrence = new HolidayRecurrence(holiday.Id, request.Recurrence.StartDate, request.Recurrence.EndDate, request.Recurrence.StartTime,
                    request.Recurrence.EndTime, request.Recurrence.RecurrenceDays.ToArray(), userId.ToString(), DateTime.UtcNow);
                await _holidayRecurrenceRepo.AddAsync(recurrence);
                holiday.HolidayRecurrences.Add(recurrence);
            }
            await _holidayRepo.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid> { Id = holiday.Id, Message = "Holiday Created Successfully" };
        }

        public async Task<List<HolidayResponseDto>> GetAllHolidaysAsync(GetAllHolidaysRequestDto request)
        {
            // filter only doctors (roleId = 5)
            var predicates = new List<Expression<Func<Holiday, bool>>>
            {
                h => h.IsActive && (h.User.RoleId == 5 || h.User.RoleId ==3 ||  h.User.RoleId == 4)
            };

            // doctorIds filter
            if (request.DoctorIds != null && request.DoctorIds.Any())
            {
                predicates.Add(h => request.DoctorIds.Contains(h.UserId));
            }

            // include user, dates, and recurrences
            var holidays = await _holidayRepo.FindWithIncludeAsync(
                predicates,
                new[] { "HolidayDates", "HolidayRecurrences", "User.Timezone" },
                noTracking: true
            );

            var response = new List<HolidayResponseDto>();

            foreach (var holiday in holidays)
            {
                var userTz = holiday.User?.Timezone?.StandardName != null
                    ? TimeZoneInfo.FindSystemTimeZoneById(holiday.User.Timezone.StandardName)
                    : TimeZoneInfo.Utc;

                foreach (var date in holiday.HolidayDates)
                {
                    if (date.HolidayDateValue >= request.StartDate && date.HolidayDateValue <= request.EndDate)
                    {
                        var localStart = date.HolidayDateValue.ToDateTime(date.StartTime);
                        var localEnd = date.HolidayDateValue.ToDateTime(date.EndTime);

                        response.Add(new HolidayResponseDto
                        {
                            UserId = holiday.UserId,
                            FullName = $"{holiday?.User?.FirstName} {holiday?.User?.LastName}",
                            StartDateTime = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz),
                            EndDateTime = TimeZoneInfo.ConvertTimeToUtc(localEnd, userTz),
                            Description = holiday.Description ?? string.Empty,
                            ColorCode = holiday.User?.ColorCode
                        });
                    }
                }

                foreach (var recurrence in holiday.HolidayRecurrences)
                {
                    var current = recurrence.StartDate < request.StartDate ? request.StartDate : recurrence.StartDate;
                    var end = recurrence.EndDate > request.EndDate ? request.EndDate : recurrence.EndDate;

                    while (current <= end)
                    {
                        if (recurrence.DayOfWeeks.Contains(current.DayOfWeek))
                        {
                            var localStart = current.ToDateTime(recurrence.StartTime);
                            var localEnd = current.ToDateTime(recurrence.EndTime);

                            response.Add(new HolidayResponseDto
                            {
                                UserId = holiday.UserId,
                                FullName = $"{holiday?.User?.FirstName} {holiday?.User?.LastName}",
                                StartDateTime = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz),
                                EndDateTime = TimeZoneInfo.ConvertTimeToUtc(localEnd, userTz),
                                Description = holiday.Description ?? string.Empty,
                                ColorCode = holiday.User?.ColorCode
                            });
                        }
                        current = current.AddDays(1);
                    }
                }
            }

            return response.ToList();
        }
    }
}
