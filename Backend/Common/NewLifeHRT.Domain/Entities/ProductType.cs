using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductType : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
        {
            public void Configure(EntityTypeBuilder<ProductType> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(p => p.Name)
                    .HasMaxLength(100)
                    .IsRequired();
            }
        }
    }
}
