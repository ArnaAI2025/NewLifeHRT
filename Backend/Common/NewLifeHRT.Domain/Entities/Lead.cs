using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Enums;
using System;

namespace NewLifeHRT.Domain.Entities
{
    public class Lead : BaseEntity<Guid>
    {
        public string Subject { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? HighLevelOwner { get; set; } 
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }
        public int OwnerId { get; set; }
        public bool? IsQualified { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<BatchMessageRecipient> BatchMessageRecipients { get; set; } = new List<BatchMessageRecipient>();
        public virtual ICollection<LeadReminder> LeadReminders { get; set; } = new List<LeadReminder>();


        public class LeadConfiguration : IEntityTypeConfiguration<Lead>
        {
            public void Configure(EntityTypeBuilder<Lead> builder)
            {
                builder.HasKey(l => l.Id);

                builder.Property(p => p.Gender).HasMaxLength(10).HasConversion<string>();

                builder.Property(p => p.IsQualified).IsRequired(false);

                builder.Property(l => l.Subject)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(l => l.FirstName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(l => l.LastName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(l => l.PhoneNumber)
                       .HasMaxLength(20)
                       .IsRequired(false);

                builder.Property(l => l.Email)
                       .HasMaxLength(255)
                       .IsRequired(false);

                builder.Property(l => l.Description)
                       .HasMaxLength(1000)
                       .IsRequired(false);

                builder.Property(l => l.Tags)
                       .IsRequired(false)
                       .HasMaxLength(500);

                builder.HasOne(l => l.Address)
                       .WithMany()
                       .HasForeignKey(l => l.AddressId)
                       .IsRequired(false)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(l => l.Owner)
                       .WithMany(u => u.Leads)
                       .HasForeignKey(l => l.OwnerId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
    
}
