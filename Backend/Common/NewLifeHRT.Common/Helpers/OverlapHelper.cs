using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Helpers
{
    /// <summary>
    /// Provides utility methods for detecting overlapping time ranges in appointments or holidays.
    /// </summary>
    public static class OverlapHelper
    {
        /// <summary>
        /// Checks if two time ranges overlap.
        /// </summary>
        /// <remarks>
        /// Calculation: Overlap exists if start1 < end2 and start2 < end1.
        /// </remarks>
        public static bool IsOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
        {
            return start1 < end2 && start2 < end1;
        }

        /// <summary>
        /// Determines if any appointment overlaps with the specified time range.
        /// </summary>
        public static bool HasAppointmentOverlap(
            IEnumerable<Appointment> appointments,
            DateOnly appointmentDate,
            TimeOnly start,
            TimeOnly end)
        {
            return appointments.Any(appt =>
                appt.AppointmentDate == appointmentDate &&
                appt.Slot != null &&
                IsOverlapping(start, end, appt.Slot.StartTime, appt.Slot.EndTime)
            );
        }

        /// <summary>
        /// Determines if the given time range conflicts with any doctor holidays.
        /// </summary>
        /// <remarks>
        /// Handles both single-day and recurring holidays.
        /// </remarks>
        public static bool HasHolidayOverlap(
            IEnumerable<Holiday> holidays,
            DateOnly appointmentDate,
            TimeOnly start,
            TimeOnly end)
        {
            return holidays.Any(h =>
                // Single day holiday dates
                h.HolidayDates.Any(hd =>
                    hd.HolidayDateValue == appointmentDate &&
                    IsOverlapping(start, end, hd.StartTime, hd.EndTime)
                )
                ||
                // Recurring holidays
                h.HolidayRecurrences.Any(hr =>
                    appointmentDate >= hr.StartDate &&
                    appointmentDate <= hr.EndDate &&
                    hr.DayOfWeeks.Contains(appointmentDate.DayOfWeek) &&
                    IsOverlapping(start, end, hr.StartTime, hr.EndTime)
                )
            );
        }
    }
}
