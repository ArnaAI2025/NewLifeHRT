using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class HolidayDate : BaseEntity<Guid>
    {
        public Guid HolidayId { get; set; }
        public DateOnly HolidayDateValue { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public virtual Holiday Holiday { get; set; } = default!;

        public HolidayDate() { }
        public HolidayDate(Guid holidayId, DateOnly holidayDate, TimeOnly startTime, TimeOnly endTime, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            HolidayId = holidayId;
            HolidayDateValue = holidayDate;
            StartTime = startTime;
            EndTime = endTime;
            IsActive = true;
        }
        public class HolidayDateConfiguration : IEntityTypeConfiguration<HolidayDate>
        {
            public void Configure(EntityTypeBuilder<HolidayDate> entity)
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.HolidayId)
                  .IsRequired();

                entity.Property(e => e.HolidayDateValue)
                  .HasColumnType("date")
                  .IsRequired();

                entity.Property(e => e.StartTime)
                  .HasColumnType("time(0)")
                  .IsRequired();

                entity.Property(e => e.EndTime)
                      .HasColumnType("time(0)")
                      .IsRequired();


                entity.HasOne(e => e.Holiday)
                      .WithMany(d => d.HolidayDates)
                      .HasForeignKey(e => e.HolidayId)
                      .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
