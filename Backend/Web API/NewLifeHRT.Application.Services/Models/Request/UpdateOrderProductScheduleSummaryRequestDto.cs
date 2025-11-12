using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UpdateOrderProductScheduleSummaryRequestDto
    {
        public DateOnly StartDate { get; set; }
        public List<string> SelectedDays { get; set; } = new();
        public List<string> Times { get; set; } = new();
    }
}
