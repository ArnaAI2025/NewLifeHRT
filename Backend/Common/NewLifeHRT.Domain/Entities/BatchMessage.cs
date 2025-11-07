using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class BatchMessage : BaseEntity<Guid>
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? StatusChangedByUserId { get; set; }
        public Status? Status { get; set; }
        // public string? Notes { get; set; }
        public bool IsMail { get; set; }
        public bool IsSms { get; set; }

        public virtual ApplicationUser MessageCreatedBy { get; set; } 
        public virtual ApplicationUser ApprovedBy { get; set; }
        public virtual ICollection<BatchMessageRecipient> BatchMessageRecipients { get; set; }

        public class BatchMessageConfiguration : IEntityTypeConfiguration<BatchMessage>
        {
            public void Configure(EntityTypeBuilder<BatchMessage> builder)
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Subject).HasMaxLength(200).IsRequired(false);
                builder.Property(e => e.Message).HasMaxLength(400).IsRequired();
                builder.Property(e => e.CreatedByUserId).IsRequired();
                builder.Property(e => e.CreatedAt).IsRequired();
                //builder.Property(e => e.Notes).HasMaxLength(500).IsRequired(false);

                builder.HasOne(bm => bm.MessageCreatedBy)
                       .WithMany(u => u.CreatedBatchMessages)
                       .HasForeignKey(bm => bm.CreatedByUserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(bm => bm.ApprovedBy)
                       .WithMany(u => u.ApprovedBatchMessages)
                       .HasForeignKey(bm => bm.StatusChangedByUserId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.SetNull);

                builder.Property(p => p.Status).IsRequired(false).HasMaxLength(10).HasConversion<string>();


            }
        }
    }
}
