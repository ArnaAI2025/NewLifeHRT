using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class CourierService : BaseEntity<int>
    {
        public string Abbreviation { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public class CourierServiceConfiguration : IEntityTypeConfiguration<CourierService>
        {
            public void Configure(EntityTypeBuilder<CourierService> builder)
            {
                builder.ToTable("CourierServices");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Abbreviation)
                    .IsRequired()
                    .HasMaxLength(20);

                builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            }
        }
    }
}
