using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class LeadReminder : BaseEntity<Guid>
    {
        public Guid ReminderId { get; set; }
        public Guid LeadId { get; set; }
        public virtual Reminder Reminder { get; set; }
        public virtual Lead Lead { get; set; }

        public LeadReminder() { }
        public LeadReminder(Guid id, Guid reminderId, Guid leadId, string? createdBy, DateTime createdAt) : base(createdBy,createdAt)
        {
            Id = id;
            ReminderId = reminderId;
            LeadId = leadId;
            IsActive = true;
        }

        public class LeadReminderConfiguration : IEntityTypeConfiguration<LeadReminder>
        {
            public void Configure(EntityTypeBuilder<LeadReminder> builder)
            {
                builder.HasKey(lr => lr.Id);

                builder.HasOne(lr => lr.Reminder)
                       .WithOne(r => r.LeadReminder)
                       .HasForeignKey<LeadReminder>(lr => lr.ReminderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(lr => lr.Lead)
                       .WithMany(l => l.LeadReminders)
                       .HasForeignKey(lr => lr.LeadId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
