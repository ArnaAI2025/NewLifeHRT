using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Slot : BaseEntity<Guid>
    {
        public Guid UserServiceLinkId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public virtual UserServiceLink UserServiceLink { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public class SlotConfiguration : IEntityTypeConfiguration<Slot>
        {
            public void Configure(EntityTypeBuilder<Slot> entity)
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.StartTime)
                      .IsRequired();

                entity.Property(s => s.EndTime)
                      .IsRequired();

                entity.HasOne(s => s.UserServiceLink)
                      .WithMany(us => us.Slots)
                      .HasForeignKey(s => s.UserServiceLinkId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.Appointments)
                    .WithOne(a => a.Slot)
                    .HasForeignKey(a => a.SlotId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
