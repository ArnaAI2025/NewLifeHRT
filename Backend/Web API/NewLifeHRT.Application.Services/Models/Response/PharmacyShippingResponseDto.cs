using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmacyShippingResponseDto
    {
        public Guid Id { get; set; }
        public int ShippingMethodId { get; set; }
        public string Value { get; set; }
        public decimal? Amount { get; set; }
        public decimal? CostOfShipping { get; set; }

    }
}
