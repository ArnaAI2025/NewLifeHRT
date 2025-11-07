using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Helpers
{
    public static class OverlapHelper
    {
        public static bool IsOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
        {
            return start1 < end2 && start2 < end1;
        }

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
