using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace NewLifeHRT.Domain.Entities
{
    public class MedicalRecommendation : BaseEntity<Guid>
    {
        public DateTime ConsultationDate { get; set; }
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int MedicationTypeId { get; set; }
        public string? OtherMedicationType { get; set; }
        public virtual MedicationType MedicationType { get; set; }

        public string? Title { get; set; }
        public string? PMHx { get; set; }
        public string? PSHx { get; set; }
        public string? FHx { get; set; }
        public string? Suppliments { get; set; }
        public string? Medication { get; set; }
        public string? SocialHistory { get; set; }
        public string? Allergies { get; set; }
        public string? HRT { get; set; }
        public int? FollowUpLabTestId { get; set; }
        public virtual FollowUpLabTest? FollowUpLabTest { get; set; }

        public string? Subjective { get; set; }
        public string? Objective { get; set; }
        public string? Assessment { get; set; }
        public string? Plan { get; set; }
        public string? SocialPoint { get; set; }
        public string? Notes { get; set; }
        public class MedicalRecommendationConfiguration : IEntityTypeConfiguration<MedicalRecommendation>
        {
            public void Configure(EntityTypeBuilder<MedicalRecommendation> builder)
            {
                builder.HasKey(mr => mr.Id);

                builder.Property(mr => mr.ConsultationDate)
                       .IsRequired();

                builder.Property(mr => mr.PatientId)
                       .IsRequired();

                builder.Property(mr => mr.DoctorId)
                       .IsRequired();

                builder.Property(mr => mr.MedicationTypeId)
                       .IsRequired();

                builder.Property(mr => mr.FollowUpLabTestId)
                       .IsRequired(false);

                builder.Property(mr => mr.Title)
                       .HasMaxLength(200)
                       .IsRequired(false);

                builder.Property(mr => mr.PMHx)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(mr => mr.PSHx)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(mr => mr.FHx)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(mr => mr.Suppliments)
                       .HasMaxLength(500)
                       .IsRequired(false);

                builder.Property(mr => mr.Medication)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(mr => mr.SocialHistory)
                       .HasMaxLength(500)
                       .IsRequired(false);

                builder.Property(mr => mr.Allergies)
                       .HasMaxLength(500)
                       .IsRequired(false);

                builder.Property(mr => mr.HRT)
                       .HasMaxLength(500)
                       .IsRequired(false);

                builder.Property(mr => mr.Subjective)
                       .HasMaxLength(2000)
                       .IsRequired(false);

                builder.Property(mr => mr.Objective)
                       .HasMaxLength(2000)
                       .IsRequired(false);

                builder.Property(mr => mr.Assessment)
                       .HasMaxLength(2000)
                       .IsRequired(false);

                builder.Property(mr => mr.Plan)
                       .HasMaxLength(2000)
                       .IsRequired(false);

                builder.Property(mr => mr.SocialPoint)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(mr => mr.Notes)
                       .HasMaxLength(2000)
                       .IsRequired(false);

                builder.HasOne(mr => mr.FollowUpLabTest)
                       .WithMany(u => u.MedicalRecommendations)
                       .HasForeignKey(mr => mr.FollowUpLabTestId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(mr => mr.MedicationType)
                       .WithMany(u => u.MedicalRecommendations)
                       .HasForeignKey(mr => mr.MedicationTypeId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(mr => mr.Patient)
                       .WithMany(p => p.MedicalRecommendations)
                       .HasForeignKey(mr => mr.PatientId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(mr => mr.ApplicationUser)
                       .WithMany(u => u.MedicalRecommendations)
                       .HasForeignKey(mr => mr.DoctorId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
