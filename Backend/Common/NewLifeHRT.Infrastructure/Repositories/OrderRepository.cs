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
    internal class OrderRepository : Repository<Order, ClinicDbContext>, IOrderRepository
    {
        public OrderRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }
    }
}
