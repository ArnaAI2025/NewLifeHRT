using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class ProductStrength : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string Name { get; set; }
        public string? Strengths { get; set; }
        public decimal? Price { get; set; }

        public ProductStrength() { }
        public ProductStrength(Guid productId, string name, string? strengths, decimal? price, string? createdBy, DateTime createdAt) : base(createdBy, createdAt)
        {
            ProductId = productId;
            Name = name;
            Strengths = strengths;
            Price = price;
        }
        public class ProductStrengthConfiguration : IEntityTypeConfiguration<ProductStrength>
        {
            public void Configure(EntityTypeBuilder<ProductStrength> entity)
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.Strengths)
                    .HasMaxLength(100);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Product)
                    .WithMany(p => p.ProductStrengths)
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
