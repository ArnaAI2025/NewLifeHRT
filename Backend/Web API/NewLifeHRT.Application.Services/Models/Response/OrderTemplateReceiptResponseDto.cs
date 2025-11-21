using NewLifeHRT.Infrastructure.Models.Templates;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderTemplateReceiptResponseDto : TemplateBaseModel
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
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
        public ShippingAddressTemplateResponseDto? PatientShippingAddress { get; set; }
        public ShippingAddressTemplateResponseDto? DoctorShippingAddress { get; set; }
        public bool? Signed { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Surcharge { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsScheduleDrug { get; set; }
        public string? LicenseNumber { get; set; }
        public ICollection<OrderDetailTemplateResponseDto> OrderDetails { get; set; } = new List<OrderDetailTemplateResponseDto>();
    }
    public class ShippingAddressTemplateResponseDto
    {
        public string? AddressLine1 { get; set; }
        public Guid AddressId { get; set; }
        public string? AddressType { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? StateOrProvince { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public bool? IsDefaultAddress { get; set; }
        public bool IsActive { get; set; }

    }
    public class OrderDetailTemplateResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPharmacyPriceListItemId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public bool? IsColdStorageProduct { get; set; }
        public bool? IsActive { get; set; }
        public decimal PerUnitAmount { get; set; }
        public decimal? Amount { get; set; }
        public string Protocol { get; set; }
        public string ProductType { get; set; }

    }
}
