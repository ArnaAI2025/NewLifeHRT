using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class PharmacyConfigurationData : BaseEntity<Guid>
    {
        public Guid PharmacyConfigurationId { get; set; }
        public int KeyId { get; set; }
        public string Value { get; set; }
        public virtual PharmacyConfigurationEntity PharmacyConfiguration { get; set; }
        public virtual IntegrationKey IntegrationKey { get; set; }
        public class PharmacyConfigurationDataConfiguration : IEntityTypeConfiguration<PharmacyConfigurationData>
        {
            public void Configure(EntityTypeBuilder<PharmacyConfigurationData> builder)
            {
                builder.HasKey(pcd => pcd.Id);

                builder.Property(pcd => pcd.Value)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.HasOne(pcd => pcd.PharmacyConfiguration)
                       .WithMany(pc => pc.ConfigurationData)
                       .HasForeignKey(pcd => pcd.PharmacyConfigurationId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pcd => pcd.IntegrationKey)
                       .WithMany(ik => ik.ConfigurationData)
                       .HasForeignKey(pcd => pcd.KeyId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasIndex(pcd => new { pcd.PharmacyConfigurationId, pcd.KeyId })
                       .IsUnique();
            }
        }
    }
}
