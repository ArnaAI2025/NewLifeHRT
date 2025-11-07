using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface IUserOtpRepository : IRepository<UserOtp>
    {
        Task<UserOtp> CreateOtpAsync(int userId, string email);
        Task<UserOtp?> ValidateOtpAsync(Guid otpId, string otpCode);
        Task MarkOtpAsUsedAsync(Guid otpId);
        Task ClearUserOtpsAsync(int userId);
    }
}
