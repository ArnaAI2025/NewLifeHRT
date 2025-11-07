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
    public class OrderProcessingApiTransaction : BaseEntity<Guid>
    {
        public Guid OrderProcessingApiTrackingId { get; set; }
        public string Endpoint { get; set; }
        public string Payload { get; set; }
        public OrderProcessApiTrackingStatusEnum Status { get; set; }
        public int? Sequence { get; set; }
        public string? ResponseMessage { get; set; }
        public virtual OrderProcessingApiTracking OrderProcessingApiTracking { get; set; }

        public class OrderProcessingApiTransactionConfiguration
            : IEntityTypeConfiguration<OrderProcessingApiTransaction>
        {
            public void Configure(EntityTypeBuilder<OrderProcessingApiTransaction> builder)
            {
                builder.HasKey(o => o.Id);

                builder.Property(o => o.Endpoint)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(o => o.Payload)
                       .IsRequired(false);

                builder.Property(o => o.ResponseMessage)
                    .IsRequired(false)
                    .HasColumnType("nvarchar(max)");
                    

                builder.Property(o => o.Status)
                       .HasConversion<string>()
                       .IsRequired()
                       .HasMaxLength(50);

                builder.Property(o => o.Sequence)
                       .IsRequired(false);

                builder.HasOne(o => o.OrderProcessingApiTracking)
                       .WithMany(t => t.Transactions)
                       .HasForeignKey(o => o.OrderProcessingApiTrackingId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
