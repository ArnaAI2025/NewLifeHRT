using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class CommisionRate : BaseEntity<Guid>
    {
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal? RatePercentage { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public CommisionRate() { }
        public CommisionRate(Guid productId, decimal fromAmount, decimal toAmount, decimal? ratePercentage, string userId, DateTime createAt) : base(userId, createAt)
        {
            ProductId = productId;
            FromAmount = fromAmount;
            ToAmount = toAmount;
            RatePercentage = ratePercentage;
            IsActive = true;
        }

        public class CommisionRatePriceListItemConfiguration : IEntityTypeConfiguration<CommisionRate>
        {
            public void Configure(EntityTypeBuilder<CommisionRate> entity)
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FromAmount)
                       .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ToAmount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RatePercentage)
                      .IsRequired(false)
                      .HasColumnType("decimal(18,2)");

                // Relationships

                entity.HasOne(e => e.Product)
                      .WithMany(d => d.CommisionRates)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            }
        }
    }
}
