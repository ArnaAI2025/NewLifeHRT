using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class CommissionSummaryDto
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalSurcharge { get; set; }
        public decimal TotalSyringe { get; set; }
        public decimal TotalShipping { get; set; }
        public decimal TotalCommissionAppliedAmount { get; set; }
        public decimal TotalCommissionPayable { get; set; }
        public int RecordCount { get; set; }
    }
}
