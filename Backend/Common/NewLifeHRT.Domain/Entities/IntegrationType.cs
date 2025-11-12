using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class IntegrationType : BaseEntity<int>
    {
        public string Type { get; set; }
        public virtual ICollection<IntegrationKey> IntegrationKeys { get; set; }
        public virtual ICollection<PharmacyConfigurationEntity> PharmacyConfigurations { get; set; } = new List<PharmacyConfigurationEntity>();
        public class IntegrationTypeConfiguration : IEntityTypeConfiguration<IntegrationType>
        {
            public void Configure(EntityTypeBuilder<IntegrationType> builder)
            {

                builder.HasKey(a => a.Id);

                builder.Property(a => a.Type)
                       .IsRequired()
                       .HasMaxLength(100);
            }
        }
    }
}
