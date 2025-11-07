using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.External.Models
{
    public class CommissionReportDto
    {
        public string SalesPersonName { get; set; }
        public string AMNumber { get; set; }
        public string ReportRange { get; set; }
        public IEnumerable<CommissionDetailDto> CommissionDetails { get; set; }
        public CommissionSummaryDto Summary { get; set; }

    }
}
