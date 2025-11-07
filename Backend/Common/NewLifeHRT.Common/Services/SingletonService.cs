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
        private readonly List<TempSlot> _reservedSlots = new();

        public async Task<bool> CheckOverlapAsync(int doctorId, DateOnly date, TimeOnly start, TimeOnly end, IAppointmentRepository appointmentRepo,
        IHolidayRepository holidayRepo)
        {
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

        public void ReleaseSlot(int doctorId, DateOnly date, TimeOnly start, TimeOnly end)
        {
            lock (_reservedSlots)
            {
                _reservedSlots.Add(new TempSlot
                {
                    DoctorId = doctorId,
                    AppointmentDate = date,
                    StartTime = start,
                    EndTime = end
                });
            }
        }

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
