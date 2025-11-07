using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class HolidayRepository : Repository<Holiday, ClinicDbContext>, IHolidayRepository
    {
        public HolidayRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<Holiday>> GetHolidaysForDoctorAsync(int doctorId, DateOnly date)
        {
            return await _dbSet
            .Include(h => h.HolidayDates)
            .Include(h => h.HolidayRecurrences)
            .Where(h => h.UserId == doctorId &&
                        (h.HolidayDates.Any(d => d.HolidayDateValue == date) ||
                         h.HolidayRecurrences.Any(r =>
                             date >= r.StartDate &&
                             date <= r.EndDate &&
                             r.RecurrenceDays.Contains(date.DayOfWeek.ToString())
                         )))
            .ToListAsync();
        }
    }
}
