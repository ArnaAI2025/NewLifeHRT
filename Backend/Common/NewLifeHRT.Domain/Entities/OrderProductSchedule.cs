using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class OrderProductSchedule : BaseEntity<Guid>
    {
        public Guid OrderProductScheduleSummaryId { get; set; }
        public string? TimeZone { get; set; }
        public DateTime? OccurrenceDateAndTime { get; set; }
        public int? Sequence { get; set; }
        public bool? Truncated { get; set; }
        public virtual OrderProductScheduleSummary OrderProductScheduleSummary { get; set; }

        public class OrderProductScheduleConfiguration : IEntityTypeConfiguration<OrderProductSchedule>
        {
            public void Configure(EntityTypeBuilder<OrderProductSchedule> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.TimeZone)
                       .HasMaxLength(100)
                       .IsRequired(false);

                builder.Property(x => x.OccurrenceDateAndTime)
                       .HasColumnType("datetime2")
                       .IsRequired(false);

                builder.Property(x => x.Sequence)
                       .IsRequired(false);


                builder.Property(x => x.Truncated)
                       .IsRequired(false);

                builder.HasOne(x => x.OrderProductScheduleSummary)
                        .WithMany(s => s.OrderProductSchedules)
                        .HasForeignKey(x => x.OrderProductScheduleSummaryId)
                        .OnDelete(DeleteBehavior.Cascade);

            }
        }
    }
}
