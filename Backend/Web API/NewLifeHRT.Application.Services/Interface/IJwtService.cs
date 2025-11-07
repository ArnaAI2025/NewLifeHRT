using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IJwtService
    {
        Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, ApplicationRole roleWithPermissions);
        ClaimsPrincipal? ValidateToken(string token);
        string GenerateRefreshToken();
    }
}
