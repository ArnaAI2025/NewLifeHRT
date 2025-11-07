using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PatientSelfReminder : BaseEntity<Guid>
    {
        public DateTime ReminderDateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public class PatientSelfReminderConfiguration : IEntityTypeConfiguration<PatientSelfReminder>
        {
            public void Configure(EntityTypeBuilder<PatientSelfReminder> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.ReminderDateTime)
                       .HasColumnType("datetime2")
                       .IsRequired();

                builder.Property(x => x.Description)
                       .HasMaxLength(500)
                       .IsRequired();

                builder.Property(x => x.PatientId)
                   .IsRequired();

                builder.HasOne(x => x.Patient)
                       .WithMany(p => p.PatientSelfReminders)
                       .HasForeignKey(x => x.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

    }
}
