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
    internal class CouponRepository : Repository<Coupon, ClinicDbContext>, ICouponRepository
    {
        public CouponRepository(ClinicDbContext context) : base(context) { }
        public async Task<bool> ExistAsync(string couponName, Guid? excludeId = null)
        {
            return await _dbSet.AnyAsync(u =>
                u.CouponName == couponName &&
                (!excludeId.HasValue || u.Id != excludeId.Value));
        }


    }
}
