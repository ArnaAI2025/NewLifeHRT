using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class FollowUpLabTest : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Duration { get; set; }

        public virtual ICollection<MedicalRecommendation> MedicalRecommendations { get; set; }
        public class FollowUpLabTestConfiguration : IEntityTypeConfiguration<FollowUpLabTest>
        {
            public void Configure(EntityTypeBuilder<FollowUpLabTest> builder)
            {
                builder.HasKey(flt => flt.Id);

                builder.Property(flt => flt.Code)
                       .IsRequired()
                       .HasMaxLength(50);                

                builder.Property(flt => flt.Duration)
                       .HasMaxLength(500);
                builder.HasMany(m => m.MedicalRecommendations)
                       .WithOne(mr => mr.FollowUpLabTest)
                       .HasForeignKey(mr => mr.FollowUpLabTestId)
                       .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }
}
