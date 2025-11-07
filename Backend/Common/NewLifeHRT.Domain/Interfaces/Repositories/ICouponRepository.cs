using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<bool> ExistAsync(string couponName, Guid? excludeId = null);
    }
}
