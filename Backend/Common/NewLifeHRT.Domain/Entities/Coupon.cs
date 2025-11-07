using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Entities
{
    public class Coupon : BaseEntity<Guid>
    {
        public string CouponName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Buget { get; set; }
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();


        public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
        {
            public void Configure(EntityTypeBuilder<Coupon> builder)
            {
                builder.HasKey(c => c.Id);

                builder.Property(c => c.CouponName)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(c => c.Amount)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();

                builder.Property(c => c.Percentage)
                       .HasColumnType("decimal(5,2)");

                builder.Property(c => c.ExpiryDate)
                       .IsRequired();
                builder.HasOne(c => c.User)
                   .WithMany(u => u.Coupons) 
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            }
        }
    }
}
