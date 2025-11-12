using Microsoft.AspNetCore.Identity;
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
    public class PatientRepository : Repository<Patient, ClinicDbContext>, IPatientRepository
    {
        public PatientRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }
        public async Task<bool> ExistAsync(string phoneNumber, string email)
        {
            return await _dbSet.AnyAsync(u =>
                ((!string.IsNullOrEmpty(phoneNumber) && u.PhoneNumber == phoneNumber) ||
                u.Email == email) && u.IsActive
            );
        }

        public async Task<(bool EmailExists, bool PhoneExists)> ExistAsync(string phoneNumber, string email, Guid? excludePatientId = null)
        {
            var query = _dbSet.AsQueryable();

            if (excludePatientId.HasValue)
            {
                query = query.Where(p => p.Id != excludePatientId.Value);
            }

            var emailExists = await query.AnyAsync(p => p.Email == email && p.IsActive);

            bool phoneExists = false;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                phoneExists = await query.AnyAsync(p => p.PhoneNumber == phoneNumber && p.IsActive);
            }

            return (emailExists, phoneExists);
        }


    }
}