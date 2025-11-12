using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PoolDetailMappings
    {
        public static PoolDetailResponseDto ToPoolDetailResponseDto(this PoolDetail poolDetail)
        {
            return new PoolDetailResponseDto
            {
                PoolId = poolDetail.PoolId,
                PoolDetailId = poolDetail.Id,
                CounselorId = poolDetail.CounselorId,
                CounselorName = poolDetail.Counselor?.FirstName + " " + poolDetail.Counselor?.LastName,
                FromDate = poolDetail.Pool?.FromDate ?? default,
                ToDate = poolDetail.Pool?.ToDate ?? default,
                Week = poolDetail.Pool?.Week ?? 0
            };
        }

        public static List<PoolDetailResponseDto> ToPoolDetailResponseDtoList(this IEnumerable<PoolDetail> poolDetails)
        {
            return poolDetails.Select(p => p.ToPoolDetailResponseDto()).ToList();
        }
    }
}
