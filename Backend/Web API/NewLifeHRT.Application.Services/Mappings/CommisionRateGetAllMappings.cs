using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CommisionRateGetAllMappings
    {
        public static CommisionRateGetAllResponseDto ToCommisionRateGetAllResponseDto(this CommisionRate commisionRate)
        {
            return new CommisionRateGetAllResponseDto
            {
                Id = commisionRate.Id,
                ProductId = commisionRate.ProductId,
                ProductName = commisionRate.Product.Name,
                FromAmount = commisionRate.FromAmount,
                ToAmount = commisionRate.ToAmount,
                RatePercentage = commisionRate.RatePercentage,
                Status = commisionRate.IsActive ? "Active" : "Inactive"
            };
        }

        public static List<CommisionRateGetAllResponseDto> ToCommisionRateGetAllResponseDtoList(this IEnumerable<CommisionRate> commisionRate)
        {
            return commisionRate.Select(commisionRate => commisionRate.ToCommisionRateGetAllResponseDto()).ToList();
        }
    }
}
