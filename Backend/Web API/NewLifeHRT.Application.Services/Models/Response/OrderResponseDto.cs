using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PatientId { get; set; }
        public Guid PharmacyId { get; set; }
        public int? PhysicianId { get; set; }
        public int CounselorId { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? PatientCreditCardId { get; set; }
        public Guid? PharmacyShippingMethodId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public DateTime? TherapyExpiration { get; set; }
        public DateTime? LastOfficeVisit { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? CouponDiscount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? TotalOnCommissionApplied { get; set; }
        public DateTime? CommissionGeneratedDate { get; set; }

        public decimal? DeliveryCharge { get; set; }
        public bool? IsOrderPaid { get; set; }
        public bool? IsCashPayment { get; set; }
        public bool? IsGenrateCommision { get; set; }
        public bool? IsReadyForLifeFile { get; set; }
        public DateTime? OrderPaidDate { get; set; }
        public DateTime? OrderFulFilled { get; set; }
        public string? RejectionResaon { get; set; }
        public string? Description { get; set; }
        public bool? Signed { get; set; }

        public int? Status { get; set; }
        public bool? IsPharmacyActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
        public string? PharmacyOrderNumber { get; set; }
        public int? CourierServiceId { get; set; }
        public string? TrackingNumber { get; set; }
        public bool? IsActive { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal? SettledAmount { get; set; }
        public DateTime? LastSettlementDate { get; set; }

    }
}
