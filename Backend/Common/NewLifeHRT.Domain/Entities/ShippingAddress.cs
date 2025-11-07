using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ShippingAddress : BaseEntity<Guid>
    {
        public Guid PatientId { get; set; }
        public Guid AddressId { get; set; }
        public Patient Patient { get; set; }
        public Address Address { get; set; }
        public bool? IsDefaultAddress { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
        {
            public void Configure(EntityTypeBuilder<ShippingAddress> builder)
            {
                builder.HasKey(sa => sa.Id);

                builder.HasOne(sa => sa.Patient)
                       .WithMany(p => p.ShippingAddresses)
                       .HasForeignKey(sa => sa.PatientId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(sa => sa.Address)
                       .WithMany(p => p.ShippingAddresses)
                       .HasForeignKey(sa => sa.AddressId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }

    }
}
