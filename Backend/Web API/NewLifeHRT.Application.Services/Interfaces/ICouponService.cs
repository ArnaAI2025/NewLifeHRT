using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponResponseDto>> GetAllAsync();
        Task<CouponResponseDto> GetCouponById(Guid id);
        Task<List<CouponResponseDto>> GetCoupons();
        Task<List<CouponResponseDto>> GetActiveCoupons();
        Task<CommonOperationResponseDto<Guid>> Create(CouponRequestDto couponRequestDto, int userId);
        Task<CommonOperationResponseDto<Guid>> Update( Guid id ,CouponRequestDto couponRequestDto, int userId);

        Task<BulkOperationResponseDto> BulkToggleActiveStatusAsync(IList<Guid> proposalIds, int userId, bool isActive);
        Task<BulkOperationResponseDto> BulkDeleteProposalAsync(IList<Guid> proposalIds);
    }
}
