using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class MessageContent : BaseEntity<Guid>
    {
        public Guid MessageId { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public virtual Message Message { get; set; }
        public class MessageContentConfiguration : IEntityTypeConfiguration<MessageContent>
        {
            public void Configure(EntityTypeBuilder<MessageContent> builder)
            {
                builder.HasKey(e => e.Id);

                builder.Property(e => e.Id).IsRequired();
                builder.Property(e => e.MessageId).IsRequired();
                builder.Property(e => e.ContentType).IsRequired().HasMaxLength(20);
                builder.Property(e => e.Content).IsRequired();

                builder.HasOne(mc => mc.Message)
                       .WithMany(m => m.MessageContents)
                       .HasForeignKey(mc => mc.MessageId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
