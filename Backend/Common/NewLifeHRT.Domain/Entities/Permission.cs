using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class Permission : BaseEntity<int>
    {
        public string PermissionName { get; set; }
        public string Type { get; set; }
        public string Section { get; set; }
        public SectionEnum SectionEnum { get; set; }
        public PermissionTypeEnum PermissionTypeEnum { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
        {
            public void Configure(EntityTypeBuilder<Permission> entity)
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PermissionName).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Type).HasMaxLength(50).IsRequired();
                entity.Property(p => p.Section).HasMaxLength(100).IsRequired();
                entity.Property(r => r.SectionEnum).HasConversion<string>();
                entity.Property(r => r.PermissionTypeEnum).HasConversion<string>();
            }
        }
    }
}
