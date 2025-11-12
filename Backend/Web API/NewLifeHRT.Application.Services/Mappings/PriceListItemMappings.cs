using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PriceListItemMappings
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
