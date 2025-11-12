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
    public class RefreshTokenRepository : Repository<RefreshToken, ClinicDbContext>, IRefreshTokenRepository
    {

        public RefreshTokenRepository(ClinicDbContext context) : base(context) { }

        public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = new RefreshToken("Web Browser", userId, token, DateTime.UtcNow.AddDays(7), "System", DateTime.UtcNow);
            await AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _dbSet
            .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.ExpiresAt = DateTime.UtcNow;
                await UpdateAsync(refreshToken);
            }
        }

        public async Task<RefreshToken?> ValidateRefreshTokenAsync(int userId, string token)
        {
            return await _dbSet
            .FirstOrDefaultAsync(rt =>
                rt.UserId == userId &&
                rt.Token == token &&
                rt.ExpiresAt > DateTime.UtcNow);
        }
    }
}
