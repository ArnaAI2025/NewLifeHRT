using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductWebForm : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public class ProductWebFormConfiguration : IEntityTypeConfiguration<ProductWebForm>
        {
            public void Configure(EntityTypeBuilder<ProductWebForm> entity)
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
