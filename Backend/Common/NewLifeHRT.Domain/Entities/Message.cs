using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class Message : BaseEntity<Guid>
    {
        public Guid ConversationId { get; set; }
        public int? UserId { get; set; }
        public string TwilioId { get; set; }
        public bool? IsRead { get; set; }
        public DateTime Timestamp { get; set; }
        public string Direction { get; set; }
        public bool? IsSent { get; set; } 

        public virtual Conversation Conversation { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<MessageContent> MessageContents { get; set; }


        public class MessageConfiguration : IEntityTypeConfiguration<Message>
        {
            public void Configure(EntityTypeBuilder<Message> builder)
            {
                builder.HasKey(e => e.Id);

                builder.Property(e => e.ConversationId).IsRequired();
                builder.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
                builder.Property(e => e.Timestamp).IsRequired();
                builder.Property(e => e.Direction).IsRequired().HasMaxLength(10);
                builder.Property(e => e.IsSent).IsRequired().HasMaxLength(20).HasDefaultValue(false);

                builder.HasOne(m => m.Conversation)
                       .WithMany(c => c.Messages)
                       .HasForeignKey(m => m.ConversationId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(m => m.User)
                       .WithMany(u => u.Messages)
                       .HasForeignKey(m => m.UserId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.SetNull);

                builder.HasMany(m => m.MessageContents)
                       .WithOne(mc => mc.Message)
                       .HasForeignKey(mc => mc.MessageId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
