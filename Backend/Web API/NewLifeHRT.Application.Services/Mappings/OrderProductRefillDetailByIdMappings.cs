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
    public static class OrderProductRefillDetailByIdMappings
    {
        public static OrderProductRefillDetailByIdResponseDto ToOrderProductRefillDetailByIdResponseDto(this OrderProductRefillDetail entity)
        {
            return new OrderProductRefillDetailByIdResponseDto
            {
                Id = entity.Id,
                ProductName = entity.ProductPharmacyPriceListItem?.Product?.Name ?? string.Empty,
                DaysSupply = entity.DaysSupply,
                DoseAmount = entity.DoseAmount,
                DoseUnit = entity.DoseUnit,
                FrequencyPerDay = entity.FrequencyPerDay,
                BottleSizeML = entity.BottleSizeMl,
                RefillDate = entity.RefillDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Status = entity.Status,
                Assumption = entity.Assumptions != null && entity.Assumptions.Any()
                    ? string.Join(", ", entity.Assumptions)
                    : string.Empty
            };
        }

        public static List<OrderProductRefillDetailByIdResponseDto> ToOrderProductRefillDetailByIdResponseDtoList(this IEnumerable<OrderProductRefillDetail> entities)
        {
            return entities.Select(e => e.ToOrderProductRefillDetailByIdResponseDto()).ToList();
        }
    }
}
