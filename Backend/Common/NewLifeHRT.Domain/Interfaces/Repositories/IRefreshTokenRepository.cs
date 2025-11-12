using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> CreateRefreshTokenAsync(int userId, string token);
        Task<RefreshToken?> ValidateRefreshTokenAsync(int userId, string token);
        Task RevokeRefreshTokenAsync(string token);
    }
}
