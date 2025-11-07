using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductStrengthMappings
    {
        public static ProductStrengthResponseDto ToProductStrengthResponseDto(this ProductStrength strength)
        {
            return new ProductStrengthResponseDto
            {
                Id = strength.Id,
                ProductId = strength.ProductId,
                Name = strength.Name,
                Strengths = strength.Strengths,
                Price = strength.Price,
                IsActive = strength.IsActive,
                ModifiedOn = strength.UpdatedAt ?? strength.CreatedAt
            };
        }

        public static List<ProductStrengthResponseDto> ToProductStrengthResponseDtoList(this IEnumerable<ProductStrength> strength)
        {
            return strength.Select(t => t.ToProductStrengthResponseDto()).ToList();
        }
    }
}
