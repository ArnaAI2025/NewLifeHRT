using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductMappings
    {
        public static ProductResponseDto ToProductResponseDto(this Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductID = product.ProductID,
                ParentName = product.Parent?.Name,
                Status = product.Status.StatusName,
                ModifiedOn = product.UpdatedAt ?? product.CreatedAt
            };
        }

        public static List<ProductResponseDto> ToProductResponseDtoList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToProductResponseDto()).ToList();
        }
    }
}
