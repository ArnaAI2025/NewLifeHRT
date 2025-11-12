using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProductScheduleResponseDto
    {
        public Guid OrderProductScheduleId { get; set; }
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Protocol { get; set; }
        public string TimeZone { get; set; }
        public DateTime? OccurrenceDateAndTime { get; set; }
    }
}
