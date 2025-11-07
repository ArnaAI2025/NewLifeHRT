using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPriceListItemService
    {
        Task<List<PriceListItemsGetAllResponseDto>> GetAllPriceListItemsAsync();
        Task<PriceListItemGetByIdResponseDto?> GetPriceListItemByIdAsync(Guid id);
        Task<List<PriceListItemByProductIdResponseDto>> GetPriceListItemByProductIdAsync(Guid productId);
        Task<List<PriceListItemByPharmacyIdResponseDto>> GetPriceListItemByPharmacyIdAsync(Guid pharmacyId, bool? isActiveFilter);
        Task<CreatePriceListItemResponseDto> CreatePriceListItemAsync(PriceListItemRequestDto request, int userId);
        Task<CreatePriceListItemResponseDto> UpdatePriceListItemAsync(Guid id, PriceListItemRequestDto request, int userId);
        Task DeletePriceListItemAsync(List<Guid> priceListItemIds, int userId);
        Task ActivatePriceListItemAsync(List<Guid> priceListItemIds, int userId);
        Task DeactivatePriceListItemAsync(List<Guid> priceListItemIds, int userId);
        Task<List<PriceListItemByPharmacyIdResponseDto>> GetActiveProductsPriceListItemByPharmacyIdAsync(Guid pharmacyId, bool? isActiveFilter);
        Task<Dictionary<Guid, decimal>> GetPricesByIdsAsync(List<Guid> priceListItemIds);
    }
}
