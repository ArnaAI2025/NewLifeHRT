using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace NewLifeHRT.Domain.Entities
{
    public class MedicationType : BaseEntity<int>
    {
        public string Code { get; set; }
        public string MedicationTypeName { get; set; }
        public virtual ICollection<MedicalRecommendation> MedicalRecommendations { get; set; }
        public class MedicationTypeConfiguration : IEntityTypeConfiguration<MedicationType>
        {
            public void Configure(EntityTypeBuilder<MedicationType> builder)
            {
                builder.HasKey(m => m.Id);

                builder.Property(m => m.Code)
                       .IsRequired()
                       .HasMaxLength(50);

                builder.Property(m => m.MedicationTypeName)
                       .IsRequired()
                       .HasMaxLength(100);
                builder.HasMany(m => m.MedicalRecommendations)
                        .WithOne(mr => mr.MedicationType)
                        .HasForeignKey(mr => mr.MedicationTypeId)
                        .OnDelete(DeleteBehavior.Restrict);

                builder.HasQueryFilter(m => m.IsActive);

            }
        }
    }
}
