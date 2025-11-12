using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IShippingAddressService
    {
        Task<List<ShippingAddressResponseDto>> GetAllAsync(Guid patientId);
        Task<ShippingAddressResponseDto> GetAllByIdAsync(Guid id);
        Task<CommonOperationResponseDto<Guid>> CreateAsync(ShippingAddressRequestDto requestDto, int userId, Guid? addressId, bool? setIsDefaultAddress);
        Task<CommonOperationResponseDto<Guid>> UpdateAsync(ShippingAddressRequestDto requestDto, int userId);
        Task<BulkOperationResponseDto> BulkToggleActiveAsync(IList<Guid> ids, int userId, bool isActive);
        Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> ids);
        Task<CommonOperationResponseDto<Guid>> SetDefaultAsync(Guid patientId, Guid addressId, int userId);
    }
}
