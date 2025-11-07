using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace NewLifeHRT.Domain.Entities
{
    public class CounselorNote : BaseEntity<Guid>
    {
        public Guid PatientId { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public bool IsAdminMailSent { get; set; }
        public bool IsDoctorMailSent { get; set; }

        public int CounselorId { get; set; }

        // Navigation properties
        public virtual ApplicationUser Counselor { get; set; }
        public virtual Patient Patient { get; set; }

        public class CounselorNoteConfiguration : IEntityTypeConfiguration<CounselorNote>
        {
            public void Configure(EntityTypeBuilder<CounselorNote> builder)
            {
                builder.HasKey(cn => cn.Id);

                builder.Property(cn => cn.Subject)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(cn => cn.Note)
                       .IsRequired()
                       .HasMaxLength(4000);

                builder.Property(cn => cn.IsAdminMailSent)
                       .IsRequired()
                       .HasDefaultValue(false); ;

                builder.Property(cn => cn.IsDoctorMailSent)
                       .IsRequired();

                builder.Property(cn => cn.PatientId)
                       .IsRequired();

                builder.Property(cn => cn.CounselorId)
                       .IsRequired();

                builder.HasOne(cn => cn.Patient)
                       .WithMany(p => p.CounselorNotes)
                       .HasForeignKey(cn => cn.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(cn => cn.Counselor)
                       .WithMany(u => u.CounselorNotes)
                       .HasForeignKey(cn => cn.CounselorId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
