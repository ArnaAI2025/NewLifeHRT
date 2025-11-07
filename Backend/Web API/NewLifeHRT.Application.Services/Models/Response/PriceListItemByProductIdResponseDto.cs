using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PriceListItemByProductIdResponseDto
    {
        public Guid Id { get; set; }
        public Guid PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
