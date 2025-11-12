using NewLifeHRT.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ProposalResponseDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid PharmacyId { get; set; }
        public int CounselorId { get; set; }
        public DropDownIntResponseDto Physician { get; set; }
        public Guid? CouponId { get; set; }
        public bool IsLab { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public DateTime? TherapyExpiration { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public bool? IsAddressVerified { get; set; }


        public List<ProposalDetailResponseDto> ProposalDetails { get; set; } = new();
    }
}
