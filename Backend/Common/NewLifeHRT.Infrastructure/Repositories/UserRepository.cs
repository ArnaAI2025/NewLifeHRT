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
    public class UserRepository : Repository<ApplicationUser, ClinicDbContext>, IUserRepository
    {
        public UserRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }

        public async Task<bool> ExistAsync(string userName, string email)
        {
            return await _dbSet.AnyAsync(u => (u.UserName == userName || u.Email == email) && !u.IsDeleted);
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _dbSet.Include(u => u.Address).Include(u => u.UserRoles).ToListAsync();
        }


        public async Task<ApplicationUser?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(u => u.Address).Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<ApplicationRole>> GetUserRolesWithPermissionsAsync(int userId)
        {
            var user = await _dbSet
                .Where(u => u.Id == userId)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync();

            return user?.UserRoles
                .Select(ur => ur.Role)
                .Where(role => role != null)
                .Cast<ApplicationRole>()
                .Distinct()
                .ToList() ?? new List<ApplicationRole>();
        }

        public async Task<string?> GetUserTimezoneAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Timezone.StandardName)
                .FirstOrDefaultAsync();
        }

        public async Task<(bool EmailExists, bool PhoneExists)> ExistAsync(string phoneNumber, string email, int? excludeUserId = null)
        {
            var query = _dbSet.AsQueryable();

            if (excludeUserId.HasValue)
            {
                query = query.Where(p => p.Id != excludeUserId.Value);
            }

            var emailExists = await query.AnyAsync(p => p.Email == email && !p.IsDeleted);

            bool phoneExists = false;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                phoneExists = await query.AnyAsync(p => p.PhoneNumber == phoneNumber && !p.IsDeleted);
            }

            return (emailExists, phoneExists);
        }
    }
}
