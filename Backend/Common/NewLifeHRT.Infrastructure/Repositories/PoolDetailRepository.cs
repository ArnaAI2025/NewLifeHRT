using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class PoolDetailRepository : Repository<PoolDetail, ClinicDbContext>, IPoolDetailRepository
    {
        public PoolDetailRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }
    }
}
