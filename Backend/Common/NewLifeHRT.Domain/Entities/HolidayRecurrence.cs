using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLifeHRT.Domain.Entities
{
    public class HolidayRecurrence : BaseEntity<Guid>
    {
        public Guid HolidayId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RecurrenceDays { get; set; } = default!;
        public virtual Holiday Holiday { get; set; } = default!;

        [NotMapped]
        public DayOfWeek[] DayOfWeeks
        {
            get => RecurrenceDays.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(d => Enum.Parse<DayOfWeek>(d.Trim()))
                                 .ToArray();

            set => RecurrenceDays = string.Join(",", value.Select(v => v.ToString()));
        }

        public HolidayRecurrence() { }
        public HolidayRecurrence(Guid holidayId, DateOnly startDate, DateOnly endDate,
                             TimeOnly startTime, TimeOnly endTime, DayOfWeek[] days, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            HolidayId = holidayId;
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
            DayOfWeeks = days;
            IsActive = true;
        }

        public class HolidayRecurrenceConfiguration : IEntityTypeConfiguration<HolidayRecurrence>
        {
            public void Configure(EntityTypeBuilder<HolidayRecurrence> entity)
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HolidayId)
                  .IsRequired();
                entity.Property(e => e.StartDate)
                  .HasColumnType("date")
                  .IsRequired();
                entity.Property(e => e.EndDate)
                  .HasColumnType("date")
                  .IsRequired();
                entity.Property(e => e.StartTime)
                  .HasColumnType("time(0)")
                  .IsRequired();

                entity.Property(e => e.EndTime)
                      .HasColumnType("time(0)")
                      .IsRequired();
                entity.Property(e => e.RecurrenceDays)
                  .HasMaxLength(200)
                  .IsRequired();
                entity.HasOne(e => e.Holiday)
                      .WithMany(d => d.HolidayRecurrences)
                      .HasForeignKey(e => e.HolidayId)
                      .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
