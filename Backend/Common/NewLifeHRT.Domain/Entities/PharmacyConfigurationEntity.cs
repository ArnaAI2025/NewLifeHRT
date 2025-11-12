using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PharmacyConfigurationEntity : BaseEntity<Guid>
    {
        public Guid PharmacyId { get; set; }
        public int TypeId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual IntegrationType IntegrationType { get; set; }
        public virtual ICollection<PharmacyConfigurationData> ConfigurationData { get; set; } = new List<PharmacyConfigurationData>();

        public PharmacyConfigurationEntity() { }
        public PharmacyConfigurationEntity(Guid pharmacyId, int typeId, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            Id = Guid.NewGuid();
            PharmacyId = pharmacyId;
            TypeId = typeId;
            IsActive = true;
        }
        public class PharmacyConfigurationEntityConfiguration : IEntityTypeConfiguration<PharmacyConfigurationEntity>
        {
            public void Configure(EntityTypeBuilder<PharmacyConfigurationEntity> builder)
            {

                builder.HasKey(pc => pc.Id);

                builder.HasOne(pc => pc.Pharmacy)
                   .WithOne(p => p.Configuration)
                   .HasForeignKey<PharmacyConfigurationEntity>(pc => pc.PharmacyId)
                   .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pc => pc.IntegrationType)
                       .WithMany(it => it.PharmacyConfigurations)
                       .HasForeignKey(pc => pc.TypeId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasIndex(pc => new { pc.PharmacyId, pc.TypeId })
                       .IsUnique();
            }
        }
    }
}
