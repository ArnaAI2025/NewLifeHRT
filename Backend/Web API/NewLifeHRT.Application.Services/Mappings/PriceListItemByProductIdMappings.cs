using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PriceListItemByProductIdMappings
    {
        public static PriceListItemByProductIdResponseDto ToPriceListItemsByProductIdResponseDto(this ProductPharmacyPriceListItem priceListItem)
        {
            return new PriceListItemByProductIdResponseDto
            {
                Id = priceListItem.Id,
                PharmacyId = priceListItem.PharmacyId,
                PharmacyName = priceListItem.Pharmacy.Name,
                Amount = priceListItem.Amount,
                Status = priceListItem.IsActive ? "Active" : "Inactive"
            };
        }

        public static List<PriceListItemByProductIdResponseDto> ToPriceListItemsByProductIdResponseDtoList(this IEnumerable<ProductPharmacyPriceListItem> priceListItem)
        {
            return priceListItem.Select(priceListItem => priceListItem.ToPriceListItemsByProductIdResponseDto()).ToList();
        }
    }
}
