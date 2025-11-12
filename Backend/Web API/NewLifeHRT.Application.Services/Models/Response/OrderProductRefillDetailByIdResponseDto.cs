using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProductRefillDetailByIdResponseDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? DaysSupply { get; set; }
        public decimal? DoseAmount { get; set; }
        public string? DoseUnit { get; set; }
        public decimal? FrequencyPerDay { get; set; }
        public decimal? BottleSizeML { get; set; }
        public string? RefillDate { get; set; }
        public string? Status { get; set; }
        public string? Assumption { get; set; }
    }
}
