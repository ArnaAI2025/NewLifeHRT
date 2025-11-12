using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PharmacyShippingMethod : BaseEntity<Guid>
    {
        public int ShippingMethodId { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }

        public Guid PharmacyId { get; set; }
        public decimal Amount { get; set; }
        public decimal CostOfShipping { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public class PharmacyShippingMethodConfiguration : IEntityTypeConfiguration<PharmacyShippingMethod>
        {
            public void Configure(EntityTypeBuilder<PharmacyShippingMethod> builder)
            {
                builder.HasKey(psm => psm.Id); 
                                                                                              
                builder.HasOne(psm => psm.Pharmacy)
                       .WithMany(p => p.PharmacyShippingMethods)
                       .HasForeignKey(psm => psm.PharmacyId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(psm => psm.ShippingMethod)
                       .WithMany(sm => sm.PharmacyShippingMethods)
                       .HasForeignKey(psm => psm.ShippingMethodId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

    }

}
