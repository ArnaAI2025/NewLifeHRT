using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class OrderProcessingApiTrackingMappings
    {
        public static OrderProcessingErrorResponseDto ToErrorResponseDto(this OrderProcessingApiTracking entity)
        {
            return new OrderProcessingErrorResponseDto
            {
                OrderId = entity.OrderId,
                OrderName = entity.Order?.Name ?? string.Empty,
                PharmacyName = entity.Order?.Pharmacy?.Name ?? string.Empty,
                IntegrationType = entity.IntegrationType?.Type ?? string.Empty,
                Status = entity.Order?.Status.ToString() ?? string.Empty,
                Transactions = entity.Transactions
                    .Where(t => t.Status == OrderProcessingApiTrackingStatusEnum.Failed)
                    .Select(t => new ApiTransactionDto
                    {
                        Endpoint = t.Endpoint,
                        Payload = t.Payload,
                        ResponseMessage = t.ResponseMessage ?? string.Empty,
                        Status = t.Status.ToString()
                    })
                    .ToList()
            };
        }
    }
}
