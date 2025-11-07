using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductStatus : BaseEntity<int>
    {
        public int Code { get; set; }
        public string StatusName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public class ProductStatusConfiguration : IEntityTypeConfiguration<ProductStatus>
        {
            public void Configure(EntityTypeBuilder<ProductStatus> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Code).IsRequired();

                entity.Property(p => p.StatusName)
                    .HasMaxLength(50)
                    .IsRequired();
            }
        }
    }
}
