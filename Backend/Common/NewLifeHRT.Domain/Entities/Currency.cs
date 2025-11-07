using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Currency : BaseEntity<int>
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public decimal ExchangeRate { get; set; }
        public int CurrencyPrecision { get; set; }
        public char CurrencySymbol { get; set; }
        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
        public virtual ICollection<ProductPharmacyPriceListItem> PriceListItems { get; set; } = new List<ProductPharmacyPriceListItem>();
        public Currency() { }

        public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
        {
            public void Configure(EntityTypeBuilder<Currency> entity)
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(c => c.CurrencyName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.ExchangeRate)
                      .HasColumnType("decimal(18,4)");

                entity.Property(c => c.CurrencySymbol)
                      .IsRequired();

                entity.HasMany(c => c.PriceListItems)
                    .WithOne(ppli => ppli.Currency)
                    .HasForeignKey(ppli => ppli.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
