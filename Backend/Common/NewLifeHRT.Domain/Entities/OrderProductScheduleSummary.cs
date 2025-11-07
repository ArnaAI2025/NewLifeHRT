using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class OrderProductScheduleSummary : BaseEntity<Guid>
    {
        public Guid OrderDetailId { get; set; }
        public string? FrequencyType { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Days { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? TimesLocal { get; set; }

        [NotMapped]
        public DayOfWeek[] DayOfWeeks
        {
            get => Days?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => Enum.Parse<DayOfWeek>(d.Trim()))
                    .ToArray() ?? Array.Empty<DayOfWeek>();

            set => Days = string.Join(",", value.Select(v => v.ToString()));
        }

        public virtual OrderDetail OrderDetail { get; set; }
        public virtual ICollection<OrderProductSchedule> OrderProductSchedules { get; set; } = new List<OrderProductSchedule>();

        public class OrderProductScheduleSummaryConfiguration : IEntityTypeConfiguration<OrderProductScheduleSummary>
        {
            public void Configure(EntityTypeBuilder<OrderProductScheduleSummary> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.FrequencyType)
                       .HasMaxLength(50)
                       .IsRequired(false);

                builder.Property(x => x.StartDate)
                       .HasColumnType("date")
                       .IsRequired(false);

                builder.Property(x => x.EndDate)
                       .HasColumnType("date")
                       .IsRequired(false);

                builder.Property(x => x.Days)
                       .HasMaxLength(100)
                       .IsRequired(false);

                builder.Property(x => x.Status)
                    .HasMaxLength(20)
                    .IsRequired(false);

                builder.Property(x => x.TimesLocal)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);


                builder.HasOne(x => x.OrderDetail)
                       .WithOne()
                       .HasForeignKey<OrderProductScheduleSummary>(x => x.OrderDetailId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(x => x.OrderProductSchedules)
                       .WithOne(x => x.OrderProductScheduleSummary)
                       .HasForeignKey(x => x.OrderProductScheduleSummaryId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
