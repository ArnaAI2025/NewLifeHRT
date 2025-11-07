using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PharmacyOrderTracking : BaseEntity<Guid>
    {
        public CourierServicesEnum? CourierServiceName { get; set; }
        public string? TrackingNumber { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public class PharmacyOrderTrackingConfiguration : IEntityTypeConfiguration<PharmacyOrderTracking>
        {
            public void Configure(EntityTypeBuilder<PharmacyOrderTracking> builder)
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.CourierServiceName).IsRequired(false).HasConversion<string>();


                builder.Property(p => p.OrderId)
                       .IsRequired();

                builder.HasOne(p => p.Order)
                        .WithOne(o => o.PharmacyOrderTracking)
                        .HasForeignKey<PharmacyOrderTracking>(p => p.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);

            }
        }
    }
}
