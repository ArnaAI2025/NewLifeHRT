using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CommisionRateByProductIdResponseDto
    {
        public Guid Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal? RatePercentage { get; set; }
        public string Status { get; set; }
    }
}
