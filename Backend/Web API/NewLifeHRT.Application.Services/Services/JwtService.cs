using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewLifeHRT.Application.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly IMultiTenantContextAccessor<MultiTenantInfo> _tenantAccessor;
        private readonly JwtSettings _jwtSettings;

        public JwtService(IConfiguration config, IMultiTenantContextAccessor<MultiTenantInfo> tenantAccessor, IOptions<JwtSettings> jwtSettings)
        {
            _config = config;
            _tenantAccessor = tenantAccessor;
            _jwtSettings = jwtSettings.Value;
        }

        public Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, ApplicationRole roleWithPermissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("isPatient", user.PatientId?.ToString() ?? ""),
                new Claim("tenant", _tenantAccessor.MultiTenantContext ?.TenantInfo ?.Identifier),
                new Claim("fullname", $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _tenantAccessor.MultiTenantContext ?.TenantInfo ?.HostUrl),
                new Claim(JwtRegisteredClaimNames.Aud, _tenantAccessor.MultiTenantContext ?.TenantInfo ?.JwtBearerAudience)
            };

            // Add role and permissions
            claims.Add(new Claim(ClaimTypes.Role, roleWithPermissions.Name));
            foreach (var permission in roleWithPermissions.RolePermissions)
                claims.Add(new Claim("permission", permission.Permission.PermissionName));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _jwtSettings.Key ?? throw new Exception("JWT Key not configured")));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryTime),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _tenantAccessor.MultiTenantContext?.TenantInfo?.HostUrl,
                Audience = _tenantAccessor.MultiTenantContext?.TenantInfo?.JwtBearerAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            var refreshToken = GenerateRefreshToken();

            return Task.FromResult(new TokenResponseDto(accessToken, refreshToken));

        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ValidateIssuer = false, // Validated per-tenant
                    ValidateAudience = false, // Validated per-tenant
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
