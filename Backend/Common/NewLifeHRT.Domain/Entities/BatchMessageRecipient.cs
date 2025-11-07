using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class BatchMessageRecipient : BaseEntity<Guid>
    {
        public Guid BatchMessageId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? LeadId { get; set; }
        public Status? Status { get; set; }
        public string? ErrorReason { get; set; }

        public virtual BatchMessage BatchMessage { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Lead? Lead { get; set; }

        public class BatchMessageRecipientConfiguration : IEntityTypeConfiguration<BatchMessageRecipient>
        {
            public void Configure(EntityTypeBuilder<BatchMessageRecipient> builder)
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Id).IsRequired();
                builder.Property(e => e.BatchMessageId).IsRequired();
                builder.Property(e => e.PatientId).IsRequired(false);

                builder.Property(e => e.ErrorReason).HasMaxLength(500).IsRequired(false);

                // Relationships
                builder.HasOne(re => re.BatchMessage)
                       .WithMany(bm => bm.BatchMessageRecipients)
                       .HasForeignKey(re => re.BatchMessageId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(re => re.Patient)
                       .WithMany(p => p.BatchMessageRecipients)
                       .HasForeignKey(re => re.PatientId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(re => re.Lead)
                       .WithMany(p => p.BatchMessageRecipients)
                       .HasForeignKey(re => re.LeadId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Property(p => p.Status).IsRequired(false).HasMaxLength(10).HasConversion<string>();

            }
        }
    }
}
