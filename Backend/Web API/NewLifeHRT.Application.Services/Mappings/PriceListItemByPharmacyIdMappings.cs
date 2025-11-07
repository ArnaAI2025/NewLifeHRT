using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PriceListItemByPharmacyIdMappings
    {
        public static PriceListItemByPharmacyIdResponseDto ToPriceListItemsByPharmacyIdResponseDto(this ProductPharmacyPriceListItem priceListItem)
        {
            return new PriceListItemByPharmacyIdResponseDto
            {
                Id = priceListItem.Id,
                ProductId = priceListItem.ProductId,
                ProductName = priceListItem.Product.Name,
                Amount = priceListItem.Amount,
                Status = priceListItem.IsActive ? "Active" : "Inactive",
                IsColdStorageProduct = priceListItem?.Product?.IsColdStorageProduct,
                Protocol = priceListItem?.Product?.Protocol,
                ProductType = priceListItem?.Product.Type?.Name,

            };
        }

        public static List<PriceListItemByPharmacyIdResponseDto> ToPriceListItemsByPharmacyIdResponseDtoList(this IEnumerable<ProductPharmacyPriceListItem> priceListItem)
        {
            return priceListItem.Select(priceListItem => priceListItem.ToPriceListItemsByPharmacyIdResponseDto()).ToList();
        }
    }
}
