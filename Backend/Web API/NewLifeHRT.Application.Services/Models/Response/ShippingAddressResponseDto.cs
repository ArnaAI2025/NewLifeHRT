using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ShippingAddressResponseDto
    {
        public string? AddressLine1 { get; set; }
        public Guid AddressId { get; set; }
        public string? AddressType { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public bool? IsDefaultAddress { get; set; }
        public bool IsActive { get; set; }

    }
}
