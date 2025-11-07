using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UpdateOrderProductRefillDetailRequestDto
    {
        public int? DaysSupply { get; set; }
        public decimal? DoseAmount { get; set; }
        public string? DoseUnit { get; set; }
        public int? FrequencyPerDay { get; set; }
        public decimal? BottleSizeMl { get; set; }
        public DateOnly? RefillDate { get; set; }
        public string? Status { get; set; }
        public string? Assumption { get; set; }

    }
}
