using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class OrderRequestDto
    {
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid? ProposalId { get; set; }
        public Guid PharmacyId { get; set; }
        public int? PhysicianId { get; set; }
        public int CounselorId { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public DateTime? TherapyExpiration { get; set; }
        public DateTime? LastOfficeVisit { get; set; }
        public int? Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? TotalOnCommissionApplied { get; set; }
        public bool? IsDeliveryChargeOverRidden { get; set; }

        public decimal? DeliveryCharge { get; set; }
        public bool? IsOrderPaid { get; set; }
        public bool? IsCashPayment { get; set; }
        public bool? IsGenrateCommision { get; set; }
        public bool? IsReadyForLifeFile { get; set; }
        public string? RejectionResaon { get; set; }
        public string? Description { get; set; }
        public DateTime? OrderPaidDate { get; set; }
        public DateTime? OrderFulFilled { get; set; }
        public bool? Signed { get; set; }
        public string? PharmacyOrderNumber { get; set; }
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new List<OrderDetailRequestDto>();
        public PharmacyOrderTrackingDto PharmacyOrderTracking { get; set; }
    }
}
