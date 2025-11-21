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
        public int ActionTypeId { get; set; }
        public int SectionId { get; set; }
        public ActionType ActionType { get; set; }
        public Section Section { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
        {
            public void Configure(EntityTypeBuilder<Permission> entity)
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PermissionName).HasMaxLength(500).IsRequired();
                entity.HasOne(p => p.ActionType)
                      .WithMany(a => a.Permissions)
                      .HasForeignKey(p => p.ActionTypeId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(p => p.Section)
                      .WithMany(s => s.Permissions)
                      .HasForeignKey(p => p.SectionId)
                      .OnDelete(DeleteBehavior.NoAction);
            }
        }
    }
}
