using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NewLifeHRT.Infrastructure.Constants;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Extensions
{
    public static class TenantConfigurationExtensions
    {
        public static IServiceCollection ConfigureTenantSpecificOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT options per tenant
            services.ConfigureAllPerTenant<JwtBearerOptions, MultiTenantInfo>((opts, tenant) =>
            {
                opts.Authority = tenant.HostUrl;
                opts.Audience = tenant.JwtBearerAudience;
                opts.ClaimsIssuer = tenant.HostUrl;
                opts.RequireHttpsMetadata = true;
                var jwtSettings = configuration.GetSection(AppSettingKeys.JWTSettings).Get<JwtSettings>();
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tenant.HostUrl,
                    ValidAudience = tenant.JwtBearerAudience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key ?? throw new Exception("JWT Key not configured")))
                };
            });

            // Configure Identity options per tenant
            services.ConfigureAllPerTenant<IdentityOptions, MultiTenantInfo>((opts, tenant) =>
            {
                if (tenant.IdentityOptions?.Password != null)
                {
                    opts.Password.RequiredLength = tenant.IdentityOptions.Password.RequiredLength;
                    opts.Password.RequireDigit = tenant.IdentityOptions.Password.RequireDigit;
                    opts.Password.RequireLowercase = tenant.IdentityOptions.Password.RequireLowercase;
                    opts.Password.RequireUppercase = tenant.IdentityOptions.Password.RequireUppercase;
                    opts.Password.RequireNonAlphanumeric = tenant.IdentityOptions.Password.RequireNonAlphanumeric;
                }

                if (tenant.IdentityOptions?.Lockout != null)
                {
                    opts.Lockout.AllowedForNewUsers = tenant.IdentityOptions.Lockout.AllowedForNewUsers;
                    opts.Lockout.MaxFailedAccessAttempts = tenant.IdentityOptions.Lockout.MaxFailedAccessAttempts;
                    opts.Lockout.DefaultLockoutTimeSpan = tenant.IdentityOptions.Lockout.DefaultLockoutTimeSpan;
                }
            });

            return services;
        }
    }
}
