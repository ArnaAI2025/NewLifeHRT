using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Pharmacy : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
        public int CurrencyId { get; set; }
        public bool IsLab { get; set; }
        public bool HasFixedCommission { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<ProductPharmacyPriceListItem> PriceListItems { get; set; } = new List<ProductPharmacyPriceListItem>();
        public virtual ICollection<PharmacyShippingMethod> PharmacyShippingMethods { get; set; } = new List<PharmacyShippingMethod>();
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Order> Orders  { get; set; } = new List<Order>();
        public virtual PharmacyConfigurationEntity Configuration { get; set; }

        public Pharmacy() { }

        public Pharmacy(string name, DateOnly? startDate, DateOnly? endDate, string? description, DateTime createdAt,string createdBy, int currencyId, bool isLab, bool hasFixedCommission, decimal? commissionPercentage) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            CurrencyId = currencyId;
            HasFixedCommission = hasFixedCommission;
            CommissionPercentage = commissionPercentage;
            IsActive = true;
            IsLab = isLab;
        }

        public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
        {
            public void Configure(EntityTypeBuilder<Pharmacy> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.StartDate)
                      .HasColumnType("date");

                entity.Property(p => p.EndDate)
                      .HasColumnType("date");

                entity.Property(p => p.Description)
                      .HasMaxLength(500);

                entity.HasOne(p => p.Currency)
                  .WithMany(c => c.Pharmacies)
                  .HasForeignKey(p => p.CurrencyId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.PriceListItems)
                    .WithOne(ppli => ppli.Pharmacy)
                    .HasForeignKey(ppli => ppli.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(p => p.PharmacyShippingMethods)
                  .WithOne(psm => psm.Pharmacy)
                  .HasForeignKey(psm => psm.PharmacyId)
                  .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
