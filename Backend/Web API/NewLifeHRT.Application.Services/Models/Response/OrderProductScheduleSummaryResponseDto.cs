using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProductScheduleSummaryResponseDto
    {
        public Guid OrderProductScheduleSummaryId { get; set; }
        public string OrderName { get; set; }
        public DateTime? OrderFulfilled { get; set; }
        public string ProductName { get; set; }
        public string Protocol { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Time { get; set; }
        public string? Status { get; set; }
    }
}
