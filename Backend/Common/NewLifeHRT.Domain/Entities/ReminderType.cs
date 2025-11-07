using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ReminderType : BaseEntity<int>
    {
        public string TypeName { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

        public class ReminderTypeConfiguration : IEntityTypeConfiguration<ReminderType>
        {
            public void Configure(EntityTypeBuilder<ReminderType> builder)
            {
                builder.HasKey(rt => rt.Id);

                builder.Property(rt => rt.TypeName)
                       .IsRequired()
                       .HasMaxLength(100);
            }
        }
    }
}
