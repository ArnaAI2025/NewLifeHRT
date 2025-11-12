using Microsoft.CodeAnalysis;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PriceListItemService : IPriceListItemService
    {
        private readonly IPriceListItemRepository _priceListItemRepository;
        public PriceListItemService(IPriceListItemRepository priceListItemRepository)
        {
            _priceListItemRepository = priceListItemRepository;
        }

        public async Task ActivatePriceListItemAsync(List<Guid> priceListItemIds, int userId)
        {
            var priceListItems = (await _priceListItemRepository.FindAsync(p => priceListItemIds.Contains(p.Id))).ToList();

            if (priceListItems == null || !priceListItems.Any())
                throw new Exception("No matching price list items found for the provided IDs.");

            foreach (var priceListItem in priceListItems)
            {
                priceListItem.IsActive = true;
                priceListItem.UpdatedBy = userId.ToString();
                priceListItem.UpdatedAt = DateTime.UtcNow;
            }

            await _priceListItemRepository.BulkUpdateAsync(priceListItems);
        }

        public async Task<CreatePriceListItemResponseDto> CreatePriceListItemAsync(PriceListItemRequestDto request, int userId)
        {
            var priceListItem = new ProductPharmacyPriceListItem(
                currencyId: request.CurrencyId,
                amount: request.Amount,
                costOfProduct: request.CostOfProduct,
                lifeFilePharmacyProductId: request.LifeFilePharmacyProductId,
                lifeFielForeignPmsId: request.LifeFielForeignPmsId,
                lifeFileDrugFormId: request.LifeFileDrugFormId,
                lifeFileDrugName: request.LifeFileDrugName,
                lifeFileDrugStrength: request.LifeFileDrugStrength,
                lifeFileQuantityUnitId: request.LifeFileQuantityUnitId,
                lifeFileScheduledCodeId: request.LifeFileScheduleCodeId,
                pharmacyId: request.PharmacyId,
                productId: request.ProductId,
                createdAt: DateTime.UtcNow,
                createdBy: userId.ToString()
            );

            var created = await _priceListItemRepository.AddAsync(priceListItem);
            return new CreatePriceListItemResponseDto { Id = priceListItem.Id };
        }

        public async Task DeactivatePriceListItemAsync(List<Guid> priceListItemIds, int userId)
        {
            var priceListItems = (await _priceListItemRepository.FindAsync(p => priceListItemIds.Contains(p.Id))).ToList();

            if (priceListItems == null || !priceListItems.Any())
                throw new Exception("No matching price list items found for the provided IDs.");

            foreach (var priceListItem in priceListItems)
            {
                priceListItem.IsActive = false;
                priceListItem.UpdatedBy = userId.ToString();
                priceListItem.UpdatedAt = DateTime.UtcNow;
            }

            await _priceListItemRepository.BulkUpdateAsync(priceListItems);
        }

        public async Task DeletePriceListItemAsync(List<Guid> priceListItemIds, int userId)
        {
            var priceListItems = (await _priceListItemRepository.FindAsync(p => priceListItemIds.Contains(p.Id))).ToList();

            if (priceListItems == null || !priceListItems.Any())
                throw new Exception("No matching price list items found for the provided IDs.");

            await _priceListItemRepository.RemoveRangeAsync(priceListItems);
        }

        public async Task<List<PriceListItemsGetAllResponseDto>> GetAllPriceListItemsAsync()
        {
            var priceListItems = await _priceListItemRepository.AllWithIncludeAsync(new[] { "Product", "Pharmacy" });
            return priceListItems.ToPriceListItemsGetAllResponseDtoList();
        }

        public async Task<PriceListItemGetByIdResponseDto?> GetPriceListItemByIdAsync(Guid id)
        {
            var priceListItem = await _priceListItemRepository.GetByIdAsync(id);

            return priceListItem?.ToPriceListItemsGetByIdResponseDto();
        }

        public async Task<List<PriceListItemByPharmacyIdResponseDto>> GetPriceListItemByPharmacyIdAsync(Guid pharmacyId, bool? isActiveFilter)
        {
            var includes = new[] { "Product", "Product.Type" };


            var predicates = new List<Expression<Func<ProductPharmacyPriceListItem, bool>>>
            {
                x => x.PharmacyId == pharmacyId
            };

            if (isActiveFilter.HasValue)
            {
                predicates.Add(x => x.IsActive == isActiveFilter.Value);
            }

            var items = await _priceListItemRepository.FindWithIncludeAsync(predicates, includes);
            return items.ToPriceListItemsByPharmacyIdResponseDtoList();
        }
        public async Task<List<PriceListItemByPharmacyIdResponseDto>> GetActiveProductsPriceListItemByPharmacyIdAsync(Guid pharmacyId, bool? isActiveFilter)
        {
            var includes = new[] { "Product", "Product.Type" };

            var predicates = new List<Expression<Func<ProductPharmacyPriceListItem, bool>>>
            {
                x => x.PharmacyId == pharmacyId,
                x => x.Product.IsActive == true  
            };

            if (isActiveFilter.HasValue)
            {
                predicates.Add(x => x.IsActive == isActiveFilter.Value);
            }

            var items = await _priceListItemRepository.FindWithIncludeAsync(predicates, includes);

            return items.ToPriceListItemsByPharmacyIdResponseDtoList();
        }

        public async Task<List<PriceListItemByProductIdResponseDto>> GetPriceListItemByProductIdAsync(Guid productId)
        {
            var includes = new[] { "Pharmacy" };

            var predicates = new List<Expression<Func<ProductPharmacyPriceListItem, bool>>>
            {
                x => x.ProductId == productId
            };

            var items = await _priceListItemRepository.FindWithIncludeAsync(predicates, includes);

            return items.ToPriceListItemsByProductIdResponseDtoList();
        }

        public async Task<CreatePriceListItemResponseDto> UpdatePriceListItemAsync(Guid id, PriceListItemRequestDto request, int userId)
        {
            var priceListItem = await _priceListItemRepository.GetByIdAsync(id);

            if (priceListItem == null) throw new Exception("Price List Item not found");

            priceListItem.CurrencyId = request.CurrencyId;
            priceListItem.Amount = request.Amount;
            priceListItem.CostOfProduct = request.CostOfProduct;
            priceListItem.LifeFilePharmacyProductId = request.LifeFilePharmacyProductId;
            priceListItem.LifeFielForeignPmsId = request.LifeFielForeignPmsId;
            priceListItem.LifeFileDrugFormId = request.LifeFileDrugFormId;
            priceListItem.LifeFileDrugName = request.LifeFileDrugName;
            priceListItem.LifeFileDrugStrength = request.LifeFileDrugStrength;
            priceListItem.LifeFileQuantityUnitId = request.LifeFileQuantityUnitId;
            priceListItem.LifeFileScheduledCodeId = request.LifeFileScheduleCodeId;
            priceListItem.PharmacyId = request.PharmacyId;
            priceListItem.ProductId = request.ProductId;
            priceListItem.UpdatedAt = DateTime.UtcNow;
            priceListItem.UpdatedBy = userId.ToString();

            var updatedPharmacy = await _priceListItemRepository.UpdateAsync(priceListItem);
            return new CreatePriceListItemResponseDto { Id = priceListItem.Id };
        }
        public async Task<Dictionary<Guid, decimal>> GetPricesByIdsAsync(List<Guid> priceListItemIds)
        {
            var items = await _priceListItemRepository.FindAsync(
                p => priceListItemIds.Contains(p.Id),
                noTracking: true
            );

            return items.ToDictionary(p => p.Id, p => p.Amount);
        }
    }
}
