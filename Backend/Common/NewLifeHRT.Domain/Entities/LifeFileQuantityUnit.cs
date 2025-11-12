using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class LifeFileQuantityUnit : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductPharmacyPriceListItem> PriceListItems { get; set; } = new List<ProductPharmacyPriceListItem>();

        public class LifeFileQuantityUnitConfiguration : IEntityTypeConfiguration<LifeFileQuantityUnit>
        {
            public void Configure(EntityTypeBuilder<LifeFileQuantityUnit> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(e => e.PriceListItems)
                    .WithOne(p => p.LifeFileQuantityUnit)
                    .HasForeignKey(p => p.LifeFileQuantityUnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
