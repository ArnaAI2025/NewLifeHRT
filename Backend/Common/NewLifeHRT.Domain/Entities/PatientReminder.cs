using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PatientReminder : BaseEntity<Guid>
    {
        public Guid ReminderId { get; set; }
        public Guid PatientId { get; set; }
        public virtual Reminder Reminder { get; set; }
        public virtual Patient Patient { get; set; }

        public PatientReminder() { }
        public PatientReminder(Guid id, Guid reminderId, Guid patientId, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = id;
            ReminderId = reminderId;
            PatientId = patientId;
            IsActive = true;
        }
        public class PatientReminderConfiguration : IEntityTypeConfiguration<PatientReminder>
        {
            public void Configure(EntityTypeBuilder<PatientReminder> builder)
            {
                builder.HasKey(pr => pr.Id);

                builder.HasOne(pr => pr.Reminder)
                       .WithOne(r => r.PatientReminder)
                       .HasForeignKey<PatientReminder>(pr => pr.ReminderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pr => pr.Patient)
                       .WithMany(p => p.PatientReminders)
                       .HasForeignKey(pr => pr.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
