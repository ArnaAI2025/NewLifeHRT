using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class ProductStrengthCreateRequestDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string? Strengths { get; set; }
        public decimal? Price { get; set; }
    }
}
