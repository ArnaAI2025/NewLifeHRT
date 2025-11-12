using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewLifeHRT.Domain.Entities
{
    public class Address : BaseEntity<Guid>
    {
        public string? AddressLine1 { get; set; }
        public string? AddressType { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; } 
        public int? StateId { get; set; } 
        public State? State { get; set; } 
        public Country? Country { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();

        public Address() { }
        public Address(string? addressLine1, string? addressType, string? city, string? postalCode, int? country,int? stateId, string? createdBy, DateTime createdAt, bool isActive) : base(createdBy, createdAt)
        {
            AddressLine1 = addressLine1;
            AddressType = addressType;
            City = city;
            PostalCode = postalCode;
            CountryId = (int?)country;
            StateId = (int?)stateId;
            IsActive = isActive;
        }

        public class AddressConfiguration : IEntityTypeConfiguration<Address>
        {
            public void Configure(EntityTypeBuilder<Address> entity)
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.AddressLine1).HasMaxLength(255).IsRequired(false);
                entity.Property(a => a.City).HasMaxLength(100).IsRequired(false);
                entity.Property(a => a.PostalCode).HasMaxLength(20).IsRequired(false);
                entity.HasMany(p => p.ShippingAddresses)
                           .WithOne(sa => sa.Address)
                           .HasForeignKey(sa => sa.AddressId)
                           .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Country)
                      .WithMany(c => c.Addresses)
                      .HasForeignKey(a => a.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }
}
