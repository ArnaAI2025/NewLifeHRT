using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Reminder : BaseEntity<Guid>
    {
        public DateTime ReminderDateTime { get; set; }
        public int ReminderTypeId { get; set; }
        public string? Description { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurrenceRuleId { get; set; }
        public DateTime? RecurrenceEndDateTime { get; set; }

        public virtual ReminderType ReminderType { get; set; }
        public virtual RecurrenceRule? RecurrenceRule { get; set; }

        public virtual PatientReminder? PatientReminder { get; set; }
        public virtual LeadReminder? LeadReminder { get; set; }

        public Reminder() { }
        public Reminder(Guid id, DateTime reminderDateTime, int reminderTypeId, string? description, bool isRecurring, int? recurrenceRuleId, DateTime? recurrenceEndDateTime, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = id;
            ReminderDateTime = reminderDateTime;
            ReminderTypeId = reminderTypeId;
            Description = description;
            IsRecurring = isRecurring;
            RecurrenceRuleId = recurrenceRuleId;
            RecurrenceEndDateTime = recurrenceEndDateTime;
            IsActive = true;
        }
        public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
        {
            public void Configure(EntityTypeBuilder<Reminder> builder)
            {
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Description)
                       .HasMaxLength(500)
                       .IsRequired(false);

                builder.Property(r => r.ReminderDateTime)
                       .IsRequired();

                builder.Property(r => r.IsRecurring)
                        .IsRequired();

                builder.Property(r => r.RecurrenceEndDateTime)
                   .IsRequired(false);

                builder.HasOne(r => r.ReminderType)
                       .WithMany(rt => rt.Reminders)
                       .HasForeignKey(r => r.ReminderTypeId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(r => r.RecurrenceRule)
                       .WithMany(rr => rr.Reminders)
                       .HasForeignKey(r => r.RecurrenceRuleId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
