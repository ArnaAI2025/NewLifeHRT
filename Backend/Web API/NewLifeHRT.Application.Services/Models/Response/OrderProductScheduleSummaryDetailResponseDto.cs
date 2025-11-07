using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProductScheduleSummaryDetailResponseDto
    {
        public Guid OrderProductScheduleSummaryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public string FrequencyType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Days { get; set; } = string.Empty;
        public List<string> Times { get; set; } = new();
    }
}
