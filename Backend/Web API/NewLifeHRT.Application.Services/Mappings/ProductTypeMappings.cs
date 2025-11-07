using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductTypeMappings
    {
        public static ProductTypeResponseDto ToProductTypeResponseDto(this ProductType type)
        {
            return new ProductTypeResponseDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        public static List<ProductTypeResponseDto> ToProductTypeResponseDtoList(this IEnumerable<ProductType> types)
        {
            return types.Select(t => t.ToProductTypeResponseDto()).ToList();
        }
    }
}
