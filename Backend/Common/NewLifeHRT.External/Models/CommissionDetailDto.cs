using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class CommissionDetailDto
    {
        public string Patient { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Surcharge { get; set; }
        public decimal Syringe { get; set; }
        public decimal Shipping { get; set; }
        public decimal CommissionAppliedTotalAmount { get; set; }
        public decimal CommissionPayable { get; set; }
    }
}
