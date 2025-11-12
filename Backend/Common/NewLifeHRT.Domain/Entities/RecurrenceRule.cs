using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class RecurrenceRule : BaseEntity<int>
    {
        public string RuleName { get; set; }
        public int? IntervalDays { get; set; }
        public int? IntervalMonths { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

        public class RecurrenceRuleConfiguration : IEntityTypeConfiguration<RecurrenceRule>
        {
            public void Configure(EntityTypeBuilder<RecurrenceRule> builder)
            {
                builder.HasKey(rr => rr.Id);

                builder.Property(rr => rr.RuleName)
                       .IsRequired()
                       .HasMaxLength(100);
                builder.Property(rr => rr.IntervalDays)
                        .IsRequired(false);

                builder.Property(rr => rr.IntervalMonths)
                       .IsRequired(false);
            }
        }
    }
}
