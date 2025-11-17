using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PharmactShippingMethodRequestDto
    {
        public int shippingMethodId { get; set; }
        public Guid? Id { get; set; }
        public decimal Amount { get; set; }
        public decimal CostOfShipping { get; set; }
        public string? ServiceCode { get; set; }
    }
}
