using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Service : BaseEntity<Guid>
    {
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string DisplayName { get; set; }
        public TimeSpan? MaxDuration { get; set; }
        public virtual ICollection<UserServiceLink> UserServices { get; set; } = new List<UserServiceLink>();

        public class ServiceConfiguration : IEntityTypeConfiguration<Service>
        {
            public void Configure(EntityTypeBuilder<Service> entity)
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.ServiceName)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(s => s.ServiceType)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(s => s.DisplayName)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(s => s.MaxDuration)
                 .IsRequired(false);
            }
        }
    }
}
