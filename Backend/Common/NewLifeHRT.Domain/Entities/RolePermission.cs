using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class RolePermission : BaseEntity<Guid>
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual ApplicationRole Role { get; set; }
        public virtual Permission Permission { get; set; }

        public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
        {
            public void Configure(EntityTypeBuilder<RolePermission> entity)
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(rp => rp.Id);

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
