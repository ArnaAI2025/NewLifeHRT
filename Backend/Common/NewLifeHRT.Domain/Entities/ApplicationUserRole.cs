using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }

        public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
        {
            public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
            {
                builder.ToTable("AspNetUserRoles");

                builder.HasKey(ur => new { ur.UserId, ur.RoleId });

                builder.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
