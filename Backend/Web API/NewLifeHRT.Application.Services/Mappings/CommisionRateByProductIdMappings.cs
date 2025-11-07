using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CommisionRateByProductIdMappings
    {
        public static CommisionRateByProductIdResponseDto ToCommisionRateByProductIdResponseDto(this CommisionRate commisionRate)
        {
            return new CommisionRateByProductIdResponseDto
            {
                Id = commisionRate.Id,
                FromAmount = commisionRate.FromAmount,
                ToAmount = commisionRate.ToAmount,
                RatePercentage = commisionRate.RatePercentage,
                Status = commisionRate.IsActive ? "Active" : "Inactive"
            };
        }

        public static List<CommisionRateByProductIdResponseDto> ToCommisionRateByProductIdResponseDtoList(this IEnumerable<CommisionRate> commisionRate)
        {
            return commisionRate.Select(commisionRate => commisionRate.ToCommisionRateByProductIdResponseDto()).ToList();
        }
    }
}
