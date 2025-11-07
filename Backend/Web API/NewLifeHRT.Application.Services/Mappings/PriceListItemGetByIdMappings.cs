using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PriceListItemGetByIdMappings
    {
        public static PriceListItemGetByIdResponseDto ToPriceListItemsGetByIdResponseDto(this ProductPharmacyPriceListItem priceListItem)
        {
            return new PriceListItemGetByIdResponseDto
            {
                Id = priceListItem.Id,
                CurrencyId = priceListItem.CurrencyId,
                Amount = priceListItem.Amount,
                CostOfProduct = priceListItem.CostOfProduct,
                LifeFilePharmacyProductId = priceListItem.LifeFilePharmacyProductId,
                LifeFielForeignPmsId = priceListItem?.LifeFielForeignPmsId,
                LifeFileDrugFormId = priceListItem?.LifeFileDrugFormId,
                LifeFileDrugName = priceListItem?.LifeFileDrugName,
                LifeFileDrugStrength = priceListItem?.LifeFileDrugStrength,
                LifeFileQuantityUnitId = priceListItem?.LifeFileQuantityUnitId,
                LifeFileScheduleCodeId = priceListItem?.LifeFileScheduledCodeId,
                PharmacyId = priceListItem.PharmacyId,
                ProductId = priceListItem.ProductId,
                Status = priceListItem.IsActive ? "Active" : "Inactive"
            };
        }

        public static List<PriceListItemGetByIdResponseDto> ToPriceListItemsGetByIdResponseDtoList(this IEnumerable<ProductPharmacyPriceListItem> priceListItem)
        {
            return priceListItem.Select(priceListItem => priceListItem.ToPriceListItemsGetByIdResponseDto()).ToList();
        }
    }
}
