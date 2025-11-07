using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderProcessingErrorResponseDto
    {
        public Guid OrderId { get; set; }
        public string OrderName { get; set; }
        public string PharmacyName { get; set; }
        public string IntegrationType { get; set; }
        public string Status { get; set; }
        public List<ApiTransactionDto> Transactions { get; set; } = new();
    }

    public class ApiTransactionDto
    {
        public string Endpoint { get; set; }
        public string Payload { get; set; }
        public string ResponseMessage { get; set; }
        public string Status { get; set; }
    }
}
