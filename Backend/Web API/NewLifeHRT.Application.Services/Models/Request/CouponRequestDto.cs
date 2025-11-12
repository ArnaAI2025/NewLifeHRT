using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CouponRequestDto
    {
        public Guid? Id { get; set; }
        public string CouponName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Buget { get; set; }         
    }
}
