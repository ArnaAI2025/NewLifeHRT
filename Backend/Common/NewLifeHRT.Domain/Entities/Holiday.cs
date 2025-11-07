using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLifeHRT.Domain.Enums;

namespace NewLifeHRT.Domain.Entities
{
    public class Holiday : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public LeaveTypeEnum LeaveTypeEnum { get; set; }
        public string? Description { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<HolidayDate> HolidayDates { get; set; }
        public virtual ICollection<HolidayRecurrence> HolidayRecurrences { get; set; }

        public Holiday() { }
        public Holiday(int userId ,LeaveTypeEnum leaveType,  string? description, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            LeaveTypeEnum = leaveType;
            Description = description;
            IsActive = true;
        }
        public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
        {
            public void Configure(EntityTypeBuilder<Holiday> entity)
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                  .IsRequired();

                entity.Property(e => e.LeaveTypeEnum).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsUnicode(true);

                entity.HasOne(e => e.User)
                 .WithMany(u => u.Holidays)
                 .HasForeignKey(e => e.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.HolidayDates)
                  .WithOne(d => d.Holiday)
                  .HasForeignKey(d => d.HolidayId)
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.HolidayRecurrences)
                  .WithOne(r => r.Holiday)
                  .HasForeignKey(r => r.HolidayId)
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.LeaveTypeEnum });
            }
        }
    }
}
