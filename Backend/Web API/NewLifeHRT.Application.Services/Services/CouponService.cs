using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<List<CouponResponseDto>> GetAllAsync()
        {
            var includes = new[] { nameof(Coupon.User) };
            var coupons = await _couponRepository.AllWithIncludeAsync(includes);
            return CouponMappings.ToCouponResponseDtoList(coupons);
        }
        public async Task<List<CouponResponseDto>> GetActiveCoupons()
        {
            var now = DateTime.UtcNow;

            var coupons = await _couponRepository.FindAsync(coupon => coupon.IsActive && coupon.ExpiryDate > now );

            if (coupons == null || !coupons.Any())
                return new List<CouponResponseDto>();

            return CouponMappings.ToCouponResponseDtoList(coupons);
        }
        public async Task<List<CouponResponseDto>> GetCoupons()
        {
            var coupons = await _couponRepository.GetAllAsync();

            if (coupons == null || !coupons.Any())
                return new List<CouponResponseDto>();

            return CouponMappings.ToCouponResponseDtoList(coupons);
        }


        public async Task<CouponResponseDto> GetCouponById(Guid id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null) return null;
            return CouponMappings.ToCouponResponseDto(coupon);
        }
        public async Task<CommonOperationResponseDto<Guid>> Create(CouponRequestDto couponRequestDto, int userId)
        {
            if (await _couponRepository.ExistAsync(couponRequestDto.CouponName))
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty, 
                    Message = "Coupon name already exists. Please choose a different name."
                };
            }

            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                CouponName = couponRequestDto.CouponName,
                ExpiryDate = couponRequestDto.ExpiryDate,
                UserId = userId,
                Amount = couponRequestDto.Amount,
                Percentage = couponRequestDto.Percentage,
                Buget = couponRequestDto.Buget,
                CreatedBy = userId.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = coupon.Id,
                Message = "Coupon created successfully."
            };
        }


        public async Task<CommonOperationResponseDto<Guid>> Update(Guid id, CouponRequestDto couponRequestDto, int userId)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);

            if (coupon == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = "Coupon not found."
                };
            }

            if (!string.Equals(coupon.CouponName, couponRequestDto.CouponName, StringComparison.OrdinalIgnoreCase))
            {
                bool exists = await _couponRepository.ExistAsync(couponRequestDto.CouponName, id);
                if (exists)
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Id = Guid.Empty,
                        Message = "Coupon name already exists. Please choose a different name."
                    };
                }
            }

            coupon.CouponName = couponRequestDto.CouponName;
            coupon.ExpiryDate = couponRequestDto.ExpiryDate;
            coupon.Amount = couponRequestDto.Amount;
            coupon.Percentage = couponRequestDto.Percentage;
            coupon.Buget = couponRequestDto.Buget;
            coupon.UpdatedBy = userId.ToString();
            coupon.UpdatedAt = DateTime.UtcNow;

            await _couponRepository.UpdateAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = coupon.Id,
                Message = "Coupon updated successfully."
            };
        }


        public async Task<BulkOperationResponseDto> BulkToggleActiveStatusAsync(IList<Guid> couponIds, int userId, bool isActive)
        {
            if (couponIds == null || !couponIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid coupon IDs provided."
                };
            }

            var couponsToUpdate = (await _couponRepository
                .FindAsync(c => couponIds.Contains(c.Id), noTracking: false))
                .ToList();

            if (!couponsToUpdate.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = couponIds.Count,
                    Message = "No coupons found for the provided IDs."
                };
            }

            foreach (var coupon in couponsToUpdate)
            {
                coupon.IsActive = isActive;
                coupon.UpdatedBy = userId.ToString();
                coupon.UpdatedAt = DateTime.UtcNow;
            }

            await _couponRepository.BulkUpdateAsync(couponsToUpdate);

            var successCount = couponsToUpdate.Count;
            var failedCount = couponIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = isActive
                    ? $"{successCount} coupon(s) activated successfully."
                    : $"{successCount} coupon(s) deactivated successfully."
            };
        }

        public async Task<BulkOperationResponseDto> BulkDeleteProposalAsync(IList<Guid> couponIds)
        {
            if (couponIds == null || !couponIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid coupon IDs provided."
                };
            }

            var couponsToDelete = (await _couponRepository.FindAsync(c => couponIds.Contains(c.Id), noTracking: false)).ToList();
            if (!couponsToDelete.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = couponIds.Count,
                    Message = "No coupons found for the provided IDs."
                };
            }
            await _couponRepository.RemoveRangeAsync(couponsToDelete);
            await _couponRepository.SaveChangesAsync();

            var successCount = couponsToDelete.Count;
            var failedCount = couponIds.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = $"{successCount} coupon(s) deleted successfully."
            };
        }
    }
}
