using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductPharmacyPriceListItem : BaseEntity<Guid>
    {
        public int? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }

        public decimal Amount { get; set; }

        public decimal? CostOfProduct { get; set; }

        public string? LifeFilePharmacyProductId { get; set; }

        public string? LifeFielForeignPmsId { get; set; }

        public int? LifeFileDrugFormId { get; set; }
        public virtual LifeFileDrugForm? LifeFileDrugForm { get; set; }

        public string? LifeFileDrugName { get; set; }

        public string? LifeFileDrugStrength { get; set; }

        public int? LifeFileQuantityUnitId { get; set; }
        public virtual LifeFileQuantityUnit? LifeFileQuantityUnit { get; set; }

        public int? LifeFileScheduledCodeId { get; set; }
        public virtual LifeFileScheduleCode? LifeFileScheduleCode { get; set; }

        public Guid PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<ProposalDetail> ProposalDetails { get; set; } = new List<ProposalDetail>();
        public virtual ICollection<OrderProductRefillDetail> OrderProductRefillDetails { get; set; } = new List<OrderProductRefillDetail>();

        public ProductPharmacyPriceListItem(int? currencyId, decimal amount, decimal? costOfProduct, string? lifeFilePharmacyProductId, string? lifeFielForeignPmsId,int? lifeFileDrugFormId,string? lifeFileDrugName, string? lifeFileDrugStrength, int? lifeFileQuantityUnitId, int? lifeFileScheduledCodeId, Guid pharmacyId,Guid productId,DateTime createdAt, string createdBy) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            CurrencyId = currencyId;
            Amount = amount;
            CostOfProduct = costOfProduct;
            LifeFilePharmacyProductId = lifeFilePharmacyProductId;
            LifeFielForeignPmsId = lifeFielForeignPmsId;
            LifeFileDrugFormId = lifeFileDrugFormId;
            LifeFileDrugName = lifeFileDrugName;
            LifeFileDrugStrength = lifeFileDrugStrength;
            LifeFileQuantityUnitId = lifeFileQuantityUnitId;
            LifeFileScheduledCodeId = lifeFileScheduledCodeId;
            PharmacyId = pharmacyId;
            ProductId = productId;
            IsActive = true;
        }

        public class ProductPharmacyPriceListItemConfiguration : IEntityTypeConfiguration<ProductPharmacyPriceListItem>
        {
            public void Configure(EntityTypeBuilder<ProductPharmacyPriceListItem> entity)
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CostOfProduct)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.LifeFilePharmacyProductId)
                      .HasMaxLength(100);

                entity.Property(e => e.LifeFielForeignPmsId)
                      .HasMaxLength(100);

                entity.Property(e => e.LifeFileDrugName)
                      .HasMaxLength(200);

                entity.Property(e => e.LifeFileDrugStrength)
                      .HasMaxLength(100);

                // Relationships
                entity.HasOne(e => e.Currency)
                      .WithMany(d=> d.PriceListItems)
                      .HasForeignKey(e => e.CurrencyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LifeFileDrugForm)
                      .WithMany(d => d.PriceListItems)
                      .HasForeignKey(e => e.LifeFileDrugFormId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LifeFileQuantityUnit)
                      .WithMany(d => d.PriceListItems)
                      .HasForeignKey(e => e.LifeFileQuantityUnitId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LifeFileScheduleCode)
                      .WithMany(d => d.PriceListItems)
                      .HasForeignKey(e => e.LifeFileScheduledCodeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Pharmacy)
                      .WithMany(d => d.PriceListItems)
                      .HasForeignKey(e => e.PharmacyId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

                entity.HasOne(e => e.Product)
                      .WithMany(d => d.PriceListItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            }
        }
    }
}
