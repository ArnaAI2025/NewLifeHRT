using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CouponMappings
    {
        public static CouponResponseDto ToCouponResponseDto(this Coupon coupon)
        {
            return new CouponResponseDto
            {
                Id = coupon.Id,
                CouponName = coupon.CouponName,
                ExpiryDate = coupon.ExpiryDate,
                CounselorId = coupon.UserId,
                CounselorName = coupon.User!= null? $"{coupon.User.FirstName} {coupon.User.LastName}".Trim(): string.Empty,
                Amount = coupon.Amount,
                Percentage = coupon.Percentage,
                Buget = coupon.Buget,
                IsActive = coupon.IsActive
            };
        }

        public static List<CouponResponseDto> ToCouponResponseDtoList(this IEnumerable<Coupon> coupons)
        {
            return coupons.Select(c => c.ToCouponResponseDto()).ToList();
        }
    }
}
