using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IAddressService
    {
        Task<CommonOperationResponseDto<Guid>> CreateAddressAsync(AddressDto addressDto, int? userId);
        Task<CommonOperationResponseDto<Guid>> UpdateAddressAsync(Guid? existingAddressId, AddressDto dto, int? userId);
        Task<BulkOperationResponseDto> BulkToggleActiveAsync(IList<Guid> ids, int userId, bool isActive);
        Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> ids);
        //Task<CommonOperationResponseDto<Guid>> SetDefaultAddress(IList<Guid> ids, Guid matchingId, int userId);
    }
}
