using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductCategoryMappings
    {
        public static ProductCategoriesResponseDto ToProductCategoriesResponseDto(this ProductCategory type)
        {
            return new ProductCategoriesResponseDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        public static List<ProductCategoriesResponseDto> ToProductCategoriesResponseDtoList(this IEnumerable<ProductCategory> categories)
        {
            return categories.Select(t => t.ToProductCategoriesResponseDto()).ToList();
        }
    }
}
