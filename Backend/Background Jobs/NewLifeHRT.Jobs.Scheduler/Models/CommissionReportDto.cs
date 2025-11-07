using NewLifeHRT.Infrastructure.Models.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Jobs.Scheduler.Models
{
    public class CommissionReportDto : TemplateBaseModel
    {
        public string SalesPersonName { get; set; }
        public string ReportRange { get; set; }
        public string? Logo { get; set; }
        public IEnumerable<CommissionDetailDto> CommissionDetails { get; set; }
        public CommissionSummaryDto Summary { get; set; }

    }
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
