using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class OrderProductRefillDetail : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }

        public string? Status { get; set; }
        public int? DaysSupply { get; set; }
        public DateOnly? RefillDate { get; set; }

        public decimal? DoseAmount { get; set; }
        public string? DoseUnit { get; set; }
        public decimal? FrequencyPerDay { get; set; }
        public decimal? FrequencyPerWeek { get; set; }
        public decimal? BottleSizeMl { get; set; }
        public decimal? VialCount { get; set; }
        public decimal? UnitsCount { get; set; }
        public decimal? ClicksPerBottle { get; set; }

        public List<string>? Assumptions { get; set; } = new();

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual ProductPharmacyPriceListItem ProductPharmacyPriceListItem { get; set; }


        public void Configure(EntityTypeBuilder<OrderProductRefillDetail> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(o => o.DaysSupply)
                   .IsRequired(false);

            builder.Property(o => o.RefillDate)
                   .HasColumnType("date")
                   .IsRequired(false);

            builder.Property(o => o.DoseAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired(false);

            builder.Property(o => o.DoseUnit)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(o => o.FrequencyPerDay)
                    .HasColumnType("decimal(18,2)")
                   .IsRequired(false);

            builder.Property(o => o.FrequencyPerWeek)
                    .HasColumnType("decimal(18,2)")
                   .IsRequired(false);

            builder.Property(o => o.BottleSizeMl)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired(false);

            builder.Property(o => o.VialCount)
                   .IsRequired(false);

            builder.Property(o => o.UnitsCount)
                   .IsRequired(false);

            builder.Property(o => o.ClicksPerBottle)
                   .IsRequired(false);

            builder.Property(o => o.Assumptions)
                   .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null))
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);

            // Relationships
            builder.HasOne(o => o.Order)
                   .WithMany(d => d.OrderProductRefillDetails)
                   .HasForeignKey(o => o.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.ProductPharmacyPriceListItem)
                   .WithMany(p => p.OrderProductRefillDetails)
                   .HasForeignKey(o => o.ProductPharmacyPriceListItemId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
