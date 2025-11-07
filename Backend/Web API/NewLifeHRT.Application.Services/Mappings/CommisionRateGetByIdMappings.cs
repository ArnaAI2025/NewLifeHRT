using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CommisionRateGetByIdMappings
    {
        public static CommisionRateGetByIdResponseDto ToCommisionRateGetByIdResponseDto(this CommisionRate commisionRate)
        {
            return new CommisionRateGetByIdResponseDto
            {
                Id = commisionRate.Id,
                ProductId = commisionRate.ProductId,
                FromAmount = commisionRate.FromAmount,
                ToAmount = commisionRate.ToAmount,
                RatePercentage = commisionRate.RatePercentage,
                Status = commisionRate.IsActive ? "Active" : "Inactive"
            };
        }

        public static List<CommisionRateGetByIdResponseDto> ToCommisionRateGetByIdResponseDtoList(this IEnumerable<CommisionRate> commisionRate)
        {
            return commisionRate.Select(commisionRate => commisionRate.ToCommisionRateGetByIdResponseDto()).ToList();
        }
    }
}
