using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PharmacyCreateRequestDto
    {
        public string Name { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsLab { get; set; }
        public bool HasFixedCommission { get; set; }
        public decimal? CommissionPercentage { get; set; }

        public int CurrencyId { get; set; }
        public string? Description { get; set; }
        public PharmactShippingMethodRequestDto[] ShippingMethods { get; set; } = null;
    }
}
