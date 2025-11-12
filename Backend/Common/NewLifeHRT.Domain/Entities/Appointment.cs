using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Appointment : BaseEntity<Guid>
    {
        public Guid SlotId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public Guid PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ModeId { get; set; }
        public int StatusId { get; set; }
        public string? Description { get; set; }

        public virtual Slot Slot { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual AppointmentMode Mode { get; set; }
        public virtual AppointmentStatus Status { get; set; }

        public Appointment() { }
        public Appointment(Guid slotId, DateOnly appointmentDate, Guid patientId, int doctorId, int modeId, int statusId, string? description, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            SlotId = slotId;
            AppointmentDate = appointmentDate;
            PatientId = patientId;
            DoctorId = doctorId;
            ModeId = modeId;
            StatusId = statusId;
            Description = description;
            IsActive = true;
        }
        public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
        {
            public void Configure(EntityTypeBuilder<Appointment> entity)
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Slot)
                      .WithMany(s => s.Appointments)
                      .HasForeignKey(a => a.SlotId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne(a => a.User)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Mode)
                      .WithMany(m => m.Appointments)
                      .HasForeignKey(a => a.ModeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Status)
                      .WithMany(st => st.Appointments)
                      .HasForeignKey(a => a.StatusId)
                      .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
