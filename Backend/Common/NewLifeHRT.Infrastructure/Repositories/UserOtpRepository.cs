using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public class UserOtpRepository : Repository<UserOtp, ClinicDbContext>, IUserOtpRepository
    {
        public UserOtpRepository(ClinicDbContext context) : base(context) { }

        public async Task ClearUserOtpsAsync(int userId)
        {
            var otps = await FindAsync(o => o.UserId == userId);
            await RemoveRangeAsync(otps);
        }

        public async Task<UserOtp> CreateOtpAsync(int userId, string email)
        {
            var otp = new UserOtp(userId, OtpHelper.GenerateSecureOtp(), DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5), false, email); 
            return await AddAsync(otp);
        }

        public async Task MarkOtpAsUsedAsync(Guid otpId)
        {
            var otp = await _dbSet.FindAsync(otpId);
            if (otp != null)
            {
                otp.IsUsed = true;
                await SaveChangesAsync();
            }
        }

        public async Task<UserOtp?> ValidateOtpAsync(Guid otpId, string otpCode)
        {
            var results = await FindAsync(
                o => o.Id == otpId &&
                     o.OtpCode == otpCode &&
                     !o.IsUsed &&
                     o.ExpiresAt > DateTime.UtcNow);

            return results.FirstOrDefault();
        }
    }
}
