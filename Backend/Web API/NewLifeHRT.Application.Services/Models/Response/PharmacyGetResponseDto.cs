using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmacyGetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsLab { get; set; }
        public bool HasFixedCommission { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public int CurrencyId { get; set; }
        public bool IsActive { get; set; }
        public PharmacyShippingResponseDto[]? ShippingMethods { get; set;  }
    }
}
