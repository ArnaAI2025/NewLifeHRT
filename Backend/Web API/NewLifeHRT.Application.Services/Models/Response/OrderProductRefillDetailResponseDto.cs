using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProductRefillDetailResponseDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public string CreatedAt { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Protocol { get; set; }
        public int Quantity { get; set; }
        public string? OrderFulfilledDate { get; set; }
        public string? ProductRefillDate { get; set; }
    }
}
