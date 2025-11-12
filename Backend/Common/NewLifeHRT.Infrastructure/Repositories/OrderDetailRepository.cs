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
    public class OrderDetailRepository : Repository<OrderDetail, ClinicDbContext>, IOrderDetailRepository
    {
        public OrderDetailRepository(
            ClinicDbContext context
        ) : base(context)
        {

        }
    }
}
