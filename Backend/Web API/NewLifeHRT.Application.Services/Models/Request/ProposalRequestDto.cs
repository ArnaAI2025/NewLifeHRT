using NewLifeHRT.Domain.DTOs;
using System;
using System.Collections.Generic;

namespace NewLifeHRT.Api.Requests
{
    public class ProposalRequestDto
    {
        public Guid? Id { get; set; }  
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid PharmacyId { get; set; }
        public int CounselorId { get; set; }
        public int PhyisianId { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public DateTime? TherapyExpiration { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public bool? IsAddressVerified { get; set; }

        public int Status { get; set; }
        public string? Description { get; set; }

        public List<ProposalDetailRequestDto> ProposalDetails { get; set; } = new();
    }
   
}
