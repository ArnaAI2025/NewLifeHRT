using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using NewLifeHRT.Domain.Entities.Hospital;

namespace NewLifeHRT.Infrastructure.Models.MultiTenancy
{
    public class MultiTenantInfo : TenantInfo
    {
        public MultiTenantInfo() { }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public Guid? ClientId { get; set; }
        public string SubDomain { get; set; }
        public bool IsActive { get; set; }
        public string HostUrl { get; set; }
        public string JwtBearerAudience { get; set; }
        public ClinicIdentityOptions IdentityOptions { get; set; } = new();
    }

    public class ClinicIdentityOptions
    {
        public PasswordOptionsConfig Password { get; set; } = new();
        public LockoutOptionsConfig Lockout { get; set; } = new();
    }

    public class PasswordOptionsConfig
    {
        public int RequiredLength { get; set; } = 8;
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireNonAlphanumeric { get; set; } = true;
    }

    public class LockoutOptionsConfig
    {
        public bool AllowedForNewUsers { get; set; } = true;
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(15);
    }
}
