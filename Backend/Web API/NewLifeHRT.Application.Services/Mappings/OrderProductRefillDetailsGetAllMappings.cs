using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class OrderProductRefillDetailsGetAllMappings
    {
        public static OrderProductRefillDetailResponseDto ToOrderProductRefillDetailResponseDto(this OrderProductRefillDetail entity)
        {
            return new OrderProductRefillDetailResponseDto
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                OrderName = entity.Order?.Name ?? string.Empty,
                CreatedAt = entity.CreatedAt.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                ProductId =  entity.ProductPharmacyPriceListItem.ProductId,
                ProductName = entity.ProductPharmacyPriceListItem?.Product?.Name ?? string.Empty,
                Protocol = entity.Order?.OrderDetails
                    ?.FirstOrDefault(od => od.ProductPharmacyPriceListItemId == entity.ProductPharmacyPriceListItemId)
                    ?.Protocol ?? string.Empty,
                Quantity = entity.Order?.OrderDetails
                    ?.FirstOrDefault(od => od.ProductPharmacyPriceListItemId == entity.ProductPharmacyPriceListItemId)
                    ?.Quantity ?? 0,
                OrderFulfilledDate = entity.Order?.OrderFulFilled?.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                ProductRefillDate = entity.RefillDate?.ToDateTime(TimeOnly.MinValue).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            };
        }

        public static List<OrderProductRefillDetailResponseDto> ToOrderProductRefillDetailResponseDtoList(this IEnumerable<OrderProductRefillDetail> entities)
        {
            return entities.Select(e => e.ToOrderProductRefillDetailResponseDto()).ToList();
        }
    }
}
