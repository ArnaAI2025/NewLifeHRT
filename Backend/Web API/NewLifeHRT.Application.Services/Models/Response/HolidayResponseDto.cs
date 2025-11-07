using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class HolidayResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
    }
}
