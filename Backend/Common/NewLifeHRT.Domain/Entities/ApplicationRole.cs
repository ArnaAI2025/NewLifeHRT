using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;
using System.Collections.Generic;

namespace NewLifeHRT.Domain.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        public AppRoleEnum RoleEnum {  get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
        {
            public void Configure(EntityTypeBuilder<ApplicationRole> entity)
            {
                entity.HasKey(x => x.Id);
                entity.Property(r => r.Name).HasMaxLength(50).IsRequired();
                entity.Property(r => r.RoleEnum).HasConversion<string>();
            }
        }

    }
}
