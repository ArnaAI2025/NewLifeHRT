using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class AppointmentMode : BaseEntity<int>
    {
        public string ModeName { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public class AppointmentModeConfiguration : IEntityTypeConfiguration<AppointmentMode>
        {
            public void Configure(EntityTypeBuilder<AppointmentMode> entity)
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.ModeName)
                      .IsRequired();
                entity.HasMany(m => m.Appointments)
                    .WithOne(a => a.Mode)
                    .HasForeignKey(a => a.ModeId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
