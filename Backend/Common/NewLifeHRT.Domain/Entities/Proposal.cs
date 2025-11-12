using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewLifeHRT.Domain.Enums;
using System;

namespace NewLifeHRT.Domain.Entities
{
    public class Proposal : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid PharmacyId { get; set; }
        public int CounselorId { get; set; }
        public int? PhysicianId { get; set; }
        public int? StatusUpdatedById { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public bool? IsAddressVerified { get; set; }
        public DateTime? TherapyExpiration { get; set; }           
        public decimal TotalAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? Surcharge { get; set; }                   
        public decimal? CouponDiscount { get; set; }                   
        public decimal? DeliveryCharge { get; set; }
        public bool? IsDeliveryChargeOverRidden { get; set; }
        public string? Description { get; set; }
        public StatusEnum? Status { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual ApplicationUser Counselor { get; set; }
        public virtual ApplicationUser Physician { get; set; }
        public virtual ApplicationUser StatusUpdatedBy { get; set; }
        public virtual Coupon Coupon { get; set; }
        public virtual PatientCreditCard PatientCreditCard { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        public virtual PharmacyShippingMethod PharmacyShippingMethod { get; set; }
        public virtual ICollection<ProposalDetail> ProposalDetails { get; set; } = new List<ProposalDetail>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
        {
            public void Configure(EntityTypeBuilder<Proposal> builder)
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                builder.Property(p => p.Subtotal)
                       .HasColumnType("decimal(18,2)");

                builder.Property(p => p.CouponDiscount)
                       .HasColumnType("decimal(18,2)");

                builder.Property(p => p.TotalAmount)
                       .HasColumnType("decimal(18,2)");

                builder.Property(p => p.Surcharge)
                       .HasColumnType("decimal(18,2)");

                builder.Property(p => p.DeliveryCharge)
                       .HasColumnType("decimal(18,2)");


                builder.HasOne(p => p.Patient)
                    .WithMany(patient => patient.Proposals)
                    .HasForeignKey(p => p.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Pharmacy)
                   .WithMany(pharmacy => pharmacy.Proposals)
                   .HasForeignKey(p => p.PharmacyId)
                   .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Counselor)
                   .WithMany(counselor => counselor.Proposals)
                   .HasForeignKey(p => p.CounselorId)
                   .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Physician)
                    .WithMany(counselor => counselor.PhysicianProposals)
                    .HasForeignKey(p => p.PhysicianId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.Coupon)
                   .WithMany(coupon => coupon.Proposals)
                   .HasForeignKey(p => p.CouponId)
                   .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.PatientCreditCard)
                  .WithMany(patientCreditCard => patientCreditCard.Proposals)
                  .HasForeignKey(p => p.PatientCreditCardId)
                  .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.PharmacyShippingMethod)
                  .WithMany(pharmacyShippingMethod => pharmacyShippingMethod.Proposals)
                  .HasForeignKey(p => p.PharmacyShippingMethodId)
                  .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(p => p.ShippingAddress)
                  .WithMany(shippingAddress => shippingAddress.Proposals)
                  .HasForeignKey(p => p.ShippingAddressId)
                  .OnDelete(DeleteBehavior.Restrict);

                builder.Property(p => p.Status).IsRequired(false).HasMaxLength(50).HasConversion<string>();

                builder.HasOne(p => p.StatusUpdatedBy)
                  .WithMany(u => u.StatusUpdatedBy) 
                  .HasForeignKey(p => p.StatusUpdatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            }
        }

    }
}
