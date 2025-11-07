using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductCategory : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Category1Products { get; set; } = new List<Product>();
        public virtual ICollection<Product> Category2Products { get; set; } = new List<Product>();
        public virtual ICollection<Product> Category3Products { get; set; } = new List<Product>();
        public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
        {
            public void Configure(EntityTypeBuilder<ProductCategory> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(p => p.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasMany(c => c.Category1Products)
                    .WithOne(p => p.Category1)
                    .HasForeignKey(p => p.Category1Id)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Category2Products)
                    .WithOne(p => p.Category2)
                    .HasForeignKey(p => p.Category2Id)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(c => c.Category3Products)
                    .WithOne(p => p.Category3)
                    .HasForeignKey(p => p.Category3Id)
                    .OnDelete(DeleteBehavior.SetNull);
            }
        }

    }
}
