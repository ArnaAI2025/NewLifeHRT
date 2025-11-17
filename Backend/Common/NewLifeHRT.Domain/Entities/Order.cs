using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;

namespace NewLifeHRT.Domain.Entities
{
    public class Order : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid? ProposalId { get; set; }
        public Guid PharmacyId { get; set; }
        public int? PhysicianId { get; set; }
        public int CounselorId { get; set; }
        public int? CourierServiceId { get; set; }
        public bool? Signed { get; set; }
        public string? PharmacyOrderNumber { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public DateTime? TherapyExpiration { get; set; }
        public DateTime? LastOfficeVisit { get; set; }
        public DateTime? OrderPaidDate { get; set; }
        public DateTime? OrderFulFilled { get; set; }       
        public decimal TotalAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? TotalOnCommissionApplied { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public bool? IsDeliveryChargeOverRidden { get; set; }
        public bool? IsOrderPaid { get; set; }
        public bool? IsCashPayment { get; set; }
        public bool? IsGenrateCommision { get; set; }
        public DateTime? CommissionGeneratedDate { get; set; }
        public bool? IsReadyForLifeFile { get; set; }
        public string? RejectionResaon { get; set; }
        public string? Description { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal? SettledAmount { get; set; }
        public DateTime? LastSettlementDate { get; set; }
        public OrderStatus? Status { get; set; }
        public string? OrderNumber { get; set; }
        public string? TrackingNumber { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual CourierService CourierService { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual Coupon Coupon { get; set; }
        public virtual PatientCreditCard PatientCreditCard { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        public virtual PharmacyShippingMethod PharmacyShippingMethod { get; set; }
        public virtual ApplicationUser Counselor { get; set; }
        public virtual ApplicationUser Physician { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<CommissionsPayable> CommissionsPayables { get; set; } = new List<CommissionsPayable>();
        public virtual ICollection<OrderProductRefillDetail> OrderProductRefillDetails { get; set; } = new List<OrderProductRefillDetail>();
        public virtual Proposal Proposal { get; set; }
        public class OrderConfiguration : IEntityTypeConfiguration<Order>
        {
            public void Configure(EntityTypeBuilder<Order> builder)
            {
                builder.HasKey(o => o.Id);
                builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
                builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
                builder.Property(o => o.Surcharge).HasColumnType("decimal(18,2)");
                builder.Property(o => o.CouponDiscount).HasColumnType("decimal(18,2)");
                builder.Property(o => o.Commission).HasColumnType("decimal(18,2)");
                builder.Property(o => o.TotalOnCommissionApplied).HasColumnType("decimal(18,2)");
                builder.Property(o => o.DeliveryCharge).HasColumnType("decimal(18,2)");

                builder.Property(o => o.Status)
                       .HasConversion<string>()
                       .HasMaxLength(25)
                       .IsRequired(false);

                builder.Property(o => o.Description).HasMaxLength(1000);
                builder.Property(o => o.RejectionResaon).HasMaxLength(500);
                builder.Property(o => o.RefundAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                builder.Property(o => o.SettledAmount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);

                builder.Property(o => o.LastSettlementDate)
                    .IsRequired(false);

                builder.HasMany(o => o.OrderDetails)
                       .WithOne(od => od.Order)
                       .HasForeignKey(od => od.OrderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(o => o.Patient)
                       .WithMany(p => p.Orders)
                       .HasForeignKey(o => o.PatientId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(o => o.Pharmacy)
                       .WithMany(p => p.Orders)
                       .HasForeignKey(o => o.PharmacyId)
                       .OnDelete(DeleteBehavior.Restrict);  

                builder.HasOne(o => o.Coupon)
                       .WithMany(c => c.Orders)
                       .HasForeignKey(o => o.CouponId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(o => o.PatientCreditCard)
                       .WithMany(pcc => pcc.Orders)
                       .HasForeignKey(o => o.PatientCreditCardId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(o => o.ShippingAddress)
                       .WithMany(sa => sa.Orders)
                       .HasForeignKey(o => o.ShippingAddressId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(o => o.PharmacyShippingMethod)
                       .WithMany(psm => psm.Orders)
                       .HasForeignKey(o => o.PharmacyShippingMethodId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(o => o.Counselor)
                       .WithMany(u => u.CounselorOrders)
                       .HasForeignKey(o => o.CounselorId)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired();

                builder.HasOne(o => o.Physician)
                       .WithMany(u => u.PhysicianOrders)
                       .HasForeignKey(o => o.PhysicianId)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired(false);
                builder.HasOne(o => o.Proposal)          
                       .WithMany(p => p.Orders)          
                       .HasForeignKey(o => o.ProposalId) 
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired(false);

                builder.HasOne(o => o.CourierService)
                       .WithMany(c => c.Orders)
                       .HasForeignKey(o => o.CourierServiceId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
