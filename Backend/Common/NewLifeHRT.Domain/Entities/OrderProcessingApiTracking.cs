using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class OrderProcessingApiTracking : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public int IntegrationTypeId { get; set; }
        public OrderProcessingApiTrackingStatusEnum Status { get; set; }
        public int? Sequence { get; set; }
        public virtual Order Order { get; set; }
        public virtual IntegrationType IntegrationType { get; set; }
        public virtual ICollection<OrderProcessingApiTransaction> Transactions { get; set; } = new List<OrderProcessingApiTransaction>();

        public class OrderProcessingApiTrackingConfiguration : IEntityTypeConfiguration<OrderProcessingApiTracking>
        {
            public void Configure(EntityTypeBuilder<OrderProcessingApiTracking> builder)
            {
                builder.HasKey(o => o.Id);

                builder.Property(o => o.Status)
                       .HasConversion<string>()
                       .HasMaxLength(25)
                       .IsRequired();

                builder.Property(o => o.Sequence)
                       .IsRequired(false);

                // Relationships
                builder.HasOne(o => o.Order)
                       .WithMany()
                       .HasForeignKey(o => o.OrderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(o => o.IntegrationType)
                       .WithMany()
                       .HasForeignKey(o => o.IntegrationTypeId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
