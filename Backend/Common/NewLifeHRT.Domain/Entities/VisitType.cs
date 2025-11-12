using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class VisitType : BaseEntity<int>
    {
        public string Code { get; set; }
        public string VisitTypeName { get; set; }
        public ICollection<Patient> Patients { get; set; }

        public class VisitTypeConfiguration : IEntityTypeConfiguration<VisitType>
        {
            public void Configure(EntityTypeBuilder<VisitType> builder)
            {

                builder.HasKey(vt => vt.Id);

                builder.Property(vt => vt.Code)
                       .IsRequired()
                       .HasMaxLength(50);

                builder.Property(vt => vt.VisitTypeName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.HasMany(vt => vt.Patients)
                       .WithOne(p => p.VisitType)
                       .HasForeignKey(p => p.VisitTypeId);
            }
        }
    }
}
