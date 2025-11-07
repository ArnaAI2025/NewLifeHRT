using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PriceListItemsGetAllMappings
    {
        public static PriceListItemsGetAllResponseDto ToPriceListItemsGetAllResponseDto(this ProductPharmacyPriceListItem priceListItem)
        {
            return new PriceListItemsGetAllResponseDto
            {
                Id = priceListItem.Id,
                ProductName = priceListItem.Product.Name,
                PharmacyName = priceListItem.Pharmacy.Name,
                Amount = priceListItem.Amount,
                Status = priceListItem.IsActive ? "Active" : "Inactive",
                ProductId = priceListItem.ProductId
            };
        }

        public static List<PriceListItemsGetAllResponseDto> ToPriceListItemsGetAllResponseDtoList(this IEnumerable<ProductPharmacyPriceListItem> priceListItem)
        {
            return priceListItem.Select(priceListItem => priceListItem.ToPriceListItemsGetAllResponseDto()).ToList();
        }
    }
}
