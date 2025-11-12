using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class IntegrationKey : BaseEntity<int>
    {
        public int IntegrationTypeId { get; set; }
        public string KeyName { get; set; }
        public string Label { get; set; }
        public virtual IntegrationType IntegrationType { get; set; }
        public virtual ICollection<PharmacyConfigurationEntity> PharmacyConfigurations { get; set; } = new List<PharmacyConfigurationEntity>();
        public virtual ICollection<PharmacyConfigurationData> ConfigurationData { get; set; } = new List<PharmacyConfigurationData>();
        public class IntegrationKeyConfiguration : IEntityTypeConfiguration<IntegrationKey>
        {
            public void Configure(EntityTypeBuilder<IntegrationKey> builder)
            {

                builder.HasKey(a => a.Id);

                builder.Property(a => a.KeyName)
               .IsRequired()
               .HasMaxLength(100);

                builder.Property(a => a.Label)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.HasOne(a => a.IntegrationType)
                       .WithMany(t => t.IntegrationKeys)
                       .HasForeignKey(a => a.IntegrationTypeId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
