using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class CommisionRateRepository : Repository<CommisionRate, ClinicDbContext>, ICommisionRateRepository
    {
        public CommisionRateRepository(ClinicDbContext context) : base(context) { }
    }
}