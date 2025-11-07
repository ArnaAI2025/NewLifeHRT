using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPharmacyShippingMethodService
    {
        Task<CommonOperationResponseDto<Guid>> CreatePharmacyShippingMethodAsync(PharmactShippingMethodRequestDto[] request, Guid pharmacyId,  int userId);
        Task<CommonOperationResponseDto<Guid>> UpdatePharmacyShippingMethodAsync(PharmactShippingMethodRequestDto[] request, Guid pharmacyId, int userId);
        Task SetPharmacyShippingMethodsActivationStatusAsync(List<Guid> pharmacyIds, bool isActive, int userId);
        Task<List<PharmacyShippingMethodResponseDto>> GetAllPharmacyShippingMethod(Guid pharmacyId);
        Task<decimal?> GetShippingMethodPriceAsync(Guid pharmacyShippingMethodId);
    }
}
