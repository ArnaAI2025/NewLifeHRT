using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductDropdownMappings
    {
        public static ProductsDropdownResponseDto ToProductDropdownResponseDto(this Product product)
        {
            return new ProductsDropdownResponseDto
            {
                Id = product.Id,
                Name = product.Name
            };
        }

        public static List<ProductsDropdownResponseDto> ToProductDropdownResponseDtoList(this IEnumerable<Product> product)
        {
            return product.Select(product => product.ToProductDropdownResponseDto()).ToList();
        }
    }
}
