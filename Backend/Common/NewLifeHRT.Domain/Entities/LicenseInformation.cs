using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class LicenseInformation : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public int StateId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual State State { get; set; }

        public class LicenseInformationConfiguration : IEntityTypeConfiguration<LicenseInformation>
        {
            public void Configure(EntityTypeBuilder<LicenseInformation> builder)
            {
                builder.HasKey(li => li.Id);

                builder.Property(li => li.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.HasOne(li => li.User)
                       .WithMany(u => u.LicenseInformations)
                       .HasForeignKey(li => li.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(li => li.State)
                       .WithMany(s => s.LicenseInformations)
                       .HasForeignKey(li => li.StateId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
