using NewLifeHRT.Common.Helpers;
using NewLifeHRT.Common.Interfaces;
using NewLifeHRT.Common.Models;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Common.Services
{
    public class SingletonService : ISingletonService
    {
        // Stores temporary slot reservations in memory to prevent overlapping appointments.
        // Acts as an in-memory cache for short-term booking conflicts.
        private readonly List<TempSlotModel> _reservedSlots = new();

        /// <summary>
        /// Checks if a given doctor's appointment slot overlaps with any existing bookings, holidays,
        /// or temporarily reserved slots. This ensures time conflicts are avoided before booking.
        /// </summary>
        /// <param name="doctorId">The unique ID of the doctor.</param>
        /// <param name="date">The appointment date being checked.</param>
        /// <param name="start">Proposed appointment start time.</param>
        /// <param name="end">Proposed appointment end time.</param>
        /// <param name="appointmentRepo">Repository used to fetch existing appointments.</param>
        /// <param name="holidayRepo">Repository used to fetch doctor holidays.</param>
        /// <returns>True if an overlap is found; otherwise, false.</returns>
        public async Task<bool> CheckOverlapAsync(int doctorId, DateOnly date, TimeOnly start, TimeOnly end, IAppointmentRepository appointmentRepo,
        IHolidayRepository holidayRepo)
        {
            // ----------Check in-memory temporary slots ----------
            // Using lock to prevent race conditions since multiple threads could
            // try to read/write _reservedSlots simultaneously.
            lock (_reservedSlots)
            {
                if (_reservedSlots.Any(s =>
                    s.DoctorId == doctorId &&
                    s.AppointmentDate == date &&
                    OverlapHelper.IsOverlapping(start, end, s.StartTime, s.EndTime)))
                {
                    return true;
                }
            }

            var appointments = await appointmentRepo.FindAsync(a => a.DoctorId == doctorId && a.AppointmentDate == date);
            if (OverlapHelper.HasAppointmentOverlap(appointments, date, start, end))
                return true;

            // check holidays
            var holidays = await holidayRepo.GetHolidaysForDoctorAsync(doctorId, date);
            if (OverlapHelper.HasHolidayOverlap(holidays, date, start, end))
                return true;

            return false;
        }

        /// <summary>
        /// Temporarily reserves a slot for a doctor to avoid double-booking while an appointment
        /// creation process is ongoing. Acts like a soft lock until the booking is confirmed or released.
        /// </summary>
        public void ReleaseSlot(int doctorId, DateOnly date, TimeOnly start, TimeOnly end)
        {
            lock (_reservedSlots)
            {
                _reservedSlots.Add(new TempSlotModel
                {
                    DoctorId = doctorId,
                    AppointmentDate = date,
                    StartTime = start,
                    EndTime = end
                });
            }
        }

        /// <summary>
        /// Frees up a previously reserved slot after the appointment is finalized or cancelled.
        /// </summary>
        public void ReserveSlot(int doctorId, DateOnly date, TimeOnly start, TimeOnly end)
        {
            lock (_reservedSlots)
            {
                var slot = _reservedSlots.FirstOrDefault(s =>
                    s.DoctorId == doctorId &&
                    s.AppointmentDate == date &&
                    s.StartTime == start &&
                    s.EndTime == end);

                if (slot != null)
                    _reservedSlots.Remove(slot);
            }
        }
    }
}
