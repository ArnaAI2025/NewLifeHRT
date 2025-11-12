using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UpdateOrderPaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public bool? IsOrderPaid { get; set; }
        public bool? IsCashPayment { get; set; }
    }
}
