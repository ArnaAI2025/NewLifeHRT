using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class OrderDetail : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Protocol { get; set; }
        public decimal? Amount { get; set; }
        public decimal PerUnitAmount { get; set; }
        public bool? IsPriceOverRidden { get; set; }
        public bool? IsReadyForRefillDateCalculation { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductPharmacyPriceListItem ProductPharmacyPriceListItem { get; set; }
        public virtual ICollection<CommissionsPayablesDetail> CommissionsPayablesDetails { get; set; } = new List<CommissionsPayablesDetail>();
        public virtual OrderProductScheduleSummary OrderProductScheduleSummary { get; set; }
        public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
        {
            public void Configure(EntityTypeBuilder<OrderDetail> builder)
            {
                builder.HasKey(pd => pd.Id);

                builder.Property(pd => pd.Quantity)
                       .IsRequired();

                builder.Property(pd => pd.Amount)
                       .HasColumnType("decimal(18,2)");

                builder.Property(pd => pd.PerUnitAmount)
                       .HasColumnType("decimal(18,2)");
                builder.Property(pd => pd.IsReadyForRefillDateCalculation).IsRequired(false);

                builder.HasOne(pd => pd.Order)
                       .WithMany(p => p.OrderDetails)
                       .HasForeignKey(pd => pd.OrderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pd => pd.Product)
                        .WithMany(p => p.OrderDetails)
                        .HasForeignKey(pd => pd.ProductId)
                        .OnDelete(DeleteBehavior.Restrict);
                builder.HasOne(od => od.Order)
                        .WithMany(o => o.OrderDetails)
                        .HasForeignKey(od => od.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(pd => pd.ProductPharmacyPriceListItem)
                       .WithMany(p => p.OrderDetails)
                       .HasForeignKey(pd => pd.ProductPharmacyPriceListItemId)
                       .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }
}
