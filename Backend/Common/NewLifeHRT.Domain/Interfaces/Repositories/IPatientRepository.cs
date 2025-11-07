using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<bool> ExistAsync(string phoneNumber, string email);
        Task<(bool EmailExists, bool PhoneExists)> ExistAsync(string phoneNumber, string email, Guid? excludePatientId = null);
    }
}
