using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationRole> GetUserRoleWithPermissionsAsync(int userId);
        Task<bool> ExistAsync(string userName, string email);
        Task<string?> GetUserTimezoneAsync(int userId);
        Task<(bool EmailExists, bool PhoneExists)> ExistAsync(string phoneNumber, string email, int? excludePatientId = null);
    }
}
