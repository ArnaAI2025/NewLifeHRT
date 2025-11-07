using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ScheduleSummaryProcessing : BaseEntity<Guid>
    {
        public Guid ScheduleSummaryId { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? TimesLocal { get; set; }
        public string? Days { get; set; } = string.Empty;
        public string? Status { get; set; }
        public virtual OrderProductScheduleSummary ScheduleSummary { get; set; }

        [NotMapped]
        public DayOfWeek[] DayOfWeeks
        {
            get => Days?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => Enum.Parse<DayOfWeek>(d.Trim()))
                    .ToArray() ?? Array.Empty<DayOfWeek>();

            set => Days = string.Join(",", value.Select(v => v.ToString()));
        }

        public class ScheduleSummaryProcessingConfiguration : IEntityTypeConfiguration<ScheduleSummaryProcessing>
        {
            public void Configure(EntityTypeBuilder<ScheduleSummaryProcessing> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.StartDate)
                       .HasColumnType("date")
                       .IsRequired(false);

                builder.Property(x => x.TimesLocal)
                       .HasColumnType("nvarchar(max)")
                       .IsRequired(false);

                builder.Property(x => x.Days)
                       .HasMaxLength(100)
                       .IsRequired(false);

                builder.Property(x => x.Status)
                       .HasMaxLength(20)
                       .IsRequired(false);

                builder.HasOne(x => x.ScheduleSummary)
                       .WithMany()
                       .HasForeignKey(x => x.ScheduleSummaryId)
                       .OnDelete(DeleteBehavior.Cascade);

            }
        }

    }
}
