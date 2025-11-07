using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Timezone : BaseEntity<int>
    {
        public string StandardName { get; set; }
        public string Abbreviation { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public class TimezoneConfiguration : IEntityTypeConfiguration<Timezone>
        {
            public void Configure(EntityTypeBuilder<Timezone> entity)
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.StandardName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(t => t.Abbreviation)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.HasMany(t => t.Users)
                      .WithOne(u => u.Timezone)
                      .HasForeignKey(u => u.TimezoneId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            }

        }
    }
}
