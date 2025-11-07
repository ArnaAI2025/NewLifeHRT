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
    public class UserServiceLinkRepository : Repository<UserServiceLink, ClinicDbContext>, IUserServiceLinkRepository
    {
        public UserServiceLinkRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<UserServiceLink>> GetByUserIdAsync(int userId)
        {
            return await FindAsync(x => x.UserId == userId);
        }
    }
}
