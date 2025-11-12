using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class LeadRepository : Repository<Lead, ClinicDbContext>, ILeadRepository
    {
        private readonly ClinicDbContext _dbContext;

        public LeadRepository(ClinicDbContext context) : base(context)
        {
            _dbContext = context;
        }


    }
}
