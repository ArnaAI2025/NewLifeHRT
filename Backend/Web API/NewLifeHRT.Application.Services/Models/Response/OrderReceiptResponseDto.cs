using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderReceiptResponseDto : Infrastructure.Models.Templates.TemplateBaseModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? LastOfficeVisit { get; set; }
        public string? DrivingLicence { get; set; }
        public string? Logo { get; set; }
        public string? Signature { get; set; }
        public string? Allergies { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? Description { get; set; }
        public string? ShippingMethodName { get; set; }
        public decimal? ShippingMethodAmount { get; set; }
        public ShippingAddressResponseDto? PatientShippingAddress { get; set; }
        public ShippingAddressResponseDto? DoctorShippingAddress { get; set; }
        public bool? Signed { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
    }
}
