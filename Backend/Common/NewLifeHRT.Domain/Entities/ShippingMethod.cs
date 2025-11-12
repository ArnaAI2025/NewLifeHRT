using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ShippingMethod :BaseEntity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<PharmacyShippingMethod> PharmacyShippingMethods { get; set; } = new List<PharmacyShippingMethod>();
        public class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
        {
            public void Configure(EntityTypeBuilder<ShippingMethod> builder)
            {

                builder.HasKey(a => a.Id);

                builder.Property(a => a.Name)
                       .IsRequired()
                       .HasMaxLength(50);


                builder.HasMany(a => a.PharmacyShippingMethods)
                       .WithOne(pa => pa.ShippingMethod)
                       .HasForeignKey(pa => pa.ShippingMethodId);
            }
        }
    }
}
