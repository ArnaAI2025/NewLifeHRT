using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class Conversation : BaseEntity<Guid>
    {
        public Guid? PatientId { get; set; }
        public Guid? LeadId { get; set; }
        public String? FromPhoneNumber { get; set; }
        public bool IsFromLead
        {
            get
            {
                return LeadId != null;
            }
        }

        public virtual Patient? Patient { get; set; }
        public virtual Lead? Lead { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
        {
            public void Configure(EntityTypeBuilder<Conversation> builder)
            {
                builder.HasKey(e => e.Id);

                builder.HasMany(c => c.Messages)
                       .WithOne(m => m.Conversation)
                       .HasForeignKey(m => m.ConversationId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(c => c.Patient)
                       .WithMany(p => p.Conversations)
                       .HasForeignKey(c => c.PatientId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(c => c.Lead)
                       .WithMany(l => l.Conversations)
                       .HasForeignKey(c => c.LeadId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
