using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewLifeHRT.Application.Services.Interfaces;
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
    /// <summary>
    /// Handles JWT creation, validation, and refresh token generation.
    /// Multi-tenant aware — embeds tenant-specific claims and issuer details.
    /// </summary>
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

        /// <summary>
        /// Generates both Access Token (JWT) and Refresh Token.
        /// Includes tenant context, user identity, and role permissions.
        /// </summary>
        public Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, IEnumerable<ApplicationRole> rolesWithPermissions)
        {
            if (rolesWithPermissions == null)
            {
                throw new InvalidOperationException("Roles with permissions are required to generate tokens.");
            }

            var roleList = rolesWithPermissions
                .Where(role => role != null)
                .ToList();

            if (!roleList.Any())
            {
                throw new InvalidOperationException("Roles with permissions are required to generate tokens.");
            }

            var tenantContext = _tenantAccessor.MultiTenantContext
                ?? throw new InvalidOperationException("Tenant context is required to generate tokens.");

            var tenantInfo = tenantContext.TenantInfo
                ?? throw new InvalidOperationException("Tenant information is required to generate tokens.");

            var tenantIdentifier = tenantInfo.Identifier
                ?? throw new InvalidOperationException("Tenant identifier is required to generate tokens.");

            var tenantIssuer = tenantInfo.HostUrl
                ?? throw new InvalidOperationException("Tenant host URL is required to generate tokens.");

            var tenantAudience = tenantInfo.JwtBearerAudience
                ?? throw new InvalidOperationException("Tenant audience is required to generate tokens.");

            var userEmail = user.Email
                ?? throw new InvalidOperationException("User email is required to generate tokens.");

            // Claim Composition
            // Each claim embeds user identity and multi-tenant data into the JWT.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim("isPatient", user.PatientId?.ToString() ?? string.Empty),
                new Claim("tenant", tenantIdentifier),
                new Claim("fullname", $"{user.FirstName} {user.LastName}".Trim()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, tenantIssuer),
                new Claim(JwtRegisteredClaimNames.Aud, tenantAudience)
            };

            // Add user role and all associated permissions.
            foreach (var role in roleList)
            {
                var roleName = role?.Name;
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName!));
                }
            }

            var permissionNames = roleList
                .Where(role => role.RolePermissions != null)
                .SelectMany(role => role.RolePermissions
                    .Select(permission => permission?.Permission?.PermissionName))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var permissionName in permissionNames)
            {
                claims.Add(new Claim("permission", permissionName!));
            }

            // ---------- Token Signing ----------
            // Uses symmetric key encryption (HMAC SHA256).
            // Hardcoded algorithm ensures consistent cross-tenant validation.
            var signingKey = _jwtSettings.Key
                ?? throw new InvalidOperationException("JWT Key not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryTime),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = tenantIssuer,
                Audience = tenantAudience
            };

            // Create and serialize token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            // Generate random, secure refresh token.
            var refreshToken = GenerateRefreshToken();

            return Task.FromResult(new TokenResponseDto(accessToken, refreshToken));

        }

        /// <summary>
        /// Generates a cryptographically secure 64-byte refresh token.
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Validates a JWT token without enforcing lifetime validation.
        /// Used mainly for extracting claims during refresh token flow.
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var signingKey = _jwtSettings.Key
                    ?? throw new InvalidOperationException("JWT Key not configured.");
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
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
