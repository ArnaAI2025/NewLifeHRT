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
    public class AppointmentRepository : Repository<Appointment, ClinicDbContext>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext context) : base(context) { }

        public async Task<List<Appointment>> GetAppointmentsByDateRangeAsync(DateOnly startDate, DateOnly endDate, List<int>? doctorIds)
        {
            var query = _dbSet
                .Include(a => a.Slot)
                    .ThenInclude(s => s.UserServiceLink)
                    .ThenInclude(us => us.Service)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Counselor)
                .Include(a => a.Mode)
                .Include(a => a.Status)
                .Include(a => a.User)
                    .ThenInclude(u => u.Timezone)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate);

            if (doctorIds != null && doctorIds.Any())
            {
                query = query.Where(a => doctorIds.Contains(a.DoctorId));
            }

            return await query.AsSplitQuery().ToListAsync();
        }
    }
}
