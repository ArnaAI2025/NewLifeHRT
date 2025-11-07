using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class UpdateOrderPaymentResponseDto
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool? IsOrderPaid { get; set; }
        public bool? IsCashPayment { get; set; }
        public DateTime? OrderPaidDate { get; set; }
    }
}
