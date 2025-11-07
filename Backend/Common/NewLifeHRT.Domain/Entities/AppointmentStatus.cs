using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class AppointmentStatus : BaseEntity<int>
    {
        public string StatusName { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
        {
            public void Configure(EntityTypeBuilder<AppointmentStatus> entity)
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.StatusName)
                      .IsRequired();
                entity.HasMany(st => st.Appointments)
                    .WithOne(a => a.Status)
                    .HasForeignKey(a => a.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
