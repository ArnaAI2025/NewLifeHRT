using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    /// <summary>
    /// Handles all business logic related to holidays — creation, recurrence setup, and retrieval.
    /// </summary>
    /// <remarks>
    /// This service coordinates between multiple repositories: 
    /// - <see cref="IHolidayRepository"/> for holiday master data
    /// - <see cref="IHolidayDateRepository"/> for individual holiday dates
    /// - <see cref="IHolidayRecurrenceRepository"/> for recurring holidays
    /// </remarks>
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

        /// <summary>
        /// Creates a new holiday entry with optional date and recurrence information.
        /// </summary>
        /// <param name="request">The DTO containing holiday details including optional recurrence and date list.</param>
        /// <param name="userId">The ID of the user creating the holiday.</param>
        /// <returns>
        /// Returns a <see cref="CommonOperationResponseDto{Guid}"/> containing the created holiday ID and success message.
        /// </returns>
        /// <remarks>
        /// - This method includes multiple entity creations (Holiday, HolidayDate, and HolidayRecurrence).  
        /// - The use of <c>DateTime.UtcNow</c> ensures time consistency across time zones.  
        /// - For clarity and testability, consider separating the HolidayDate and HolidayRecurrence creation into helper methods.
        /// </remarks>
        /// 
        public async Task<CommonOperationResponseDto<Guid>> CreateHolidayAsync(
     CreateHolidayRequestDto request,
     int userId)
        {
            ArgumentNullException.ThrowIfNull(request);

            string? actor = userId.ToString();
            DateTime nowUtc = DateTime.UtcNow;

            // Base holiday
            var holiday = new Holiday(
                request.UserId,
                request.LeaveType,
                request.Description,
                actor,
                nowUtc);

            await _holidayRepo.AddAsync(holiday);

            // Holiday dates (optional)
            if (request.HolidayDates?.Any() == true)
            {
                foreach (var dateDto in request.HolidayDates)
                {
                    if (dateDto is null) continue;

                    var holidayDate = new HolidayDate(
                        holiday.Id,
                        dateDto.Date,
                        dateDto.StartTime,
                        dateDto.EndTime,
                        actor,
                        nowUtc);

                    await _holidayDateRepo.AddAsync(holidayDate);
                    holiday.HolidayDates.Add(holidayDate);
                }
            }

            // Recurrence (optional)
            if (request.Recurrence is not null)
            {
                var recurrence = new HolidayRecurrence(
                    holiday.Id,
                    request.Recurrence.StartDate,
                    request.Recurrence.EndDate,
                    request.Recurrence.StartTime,
                    request.Recurrence.EndTime,
                    request.Recurrence.RecurrenceDays?.ToArray() ?? Array.Empty<DayOfWeek>(),
                    actor,
                    nowUtc);

                await _holidayRecurrenceRepo.AddAsync(recurrence);
                holiday.HolidayRecurrences.Add(recurrence);
            }

            await _holidayRepo.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = holiday.Id,
                Message = "Holiday created successfully."
            };
        }

        /// <summary>
        /// Retrieves all holidays (including recurrence expansion) for a given filter range and doctor IDs.
        /// </summary>
        /// <param name="request">The filtering criteria containing date range and doctor IDs.</param>
        /// <returns>
        /// A list of <see cref="HolidayResponseDto"/> containing user details, holiday times, and descriptions.
        /// </returns>
        /// <remarks>
        /// This method performs multiple layers of computation:
        /// <list type="bullet">
        ///   <item><description>Filters only active users with specific Role IDs (3, 4, 5).</description></item>
        ///   <item><description>Converts local date-time values to UTC for consistency in scheduling.</description></item>
        ///   <item><description>Expands recurring holidays by iterating through all applicable days within the requested range.</description></item>
        /// </list>
        /// <para><b>Hardcoded Values:</b> The Role IDs (3, 4, 5) likely represent Receptionist, Nurse and Doctor— 
        /// consider moving these to an Enum or configuration constant for readability and maintainability.</para>
        /// <para><b>Refactoring Note:</b> The method mixes filtering, recurrence calculation, and timezone conversion. 
        /// Consider extracting recurrence expansion and DTO mapping into helper/private methods for clarity.</para>
        /// </remarks>
        public async Task<List<HolidayResponseDto>> GetAllHolidaysAsync(GetAllHolidaysRequestDto request)
        {
            // Filter: Only active doctors, nurses, or receptionist (roleId = 5, 4, 3)
            var targetedRoleIds = new[] { 5, 3, 4 };

            var predicates = new List<Expression<Func<Holiday, bool>>>
            {
                h => h.IsActive && h.User.UserRoles.Any(ur => targetedRoleIds.Contains(ur.RoleId))
            };

            // Filter by specific doctor IDs, if provided
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
                var userTz = ResolveTimeZone(holiday.User?.Timezone?.StandardName);

                // --- Process fixed (non-recurrent) holiday dates -
                foreach (var date in holiday.HolidayDates ?? Enumerable.Empty<HolidayDate>())
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

                // --- Process recurring holidays ---
                foreach (var recurrence in holiday.HolidayRecurrences ?? Enumerable.Empty<HolidayRecurrence>())
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
        private static TimeZoneInfo ResolveTimeZone(string? standardName)
        {
            if (string.IsNullOrWhiteSpace(standardName))
            {
                return TimeZoneInfo.Utc;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(standardName);
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc;
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
