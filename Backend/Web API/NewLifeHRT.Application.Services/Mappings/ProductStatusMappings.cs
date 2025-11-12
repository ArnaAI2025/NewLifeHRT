using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductStatusMappings
    {
        public static ProductStatusResponseDto ToProductStatusResponseDto(this ProductStatus type)
        {
            return new ProductStatusResponseDto
            {
                Id = type.Id,
                StatusName = type.StatusName
            };
        }

        public static List<ProductStatusResponseDto> ToProductStatusResponseDtoList(this IEnumerable<ProductStatus> status)
        {
            return status.Select(t => t.ToProductStatusResponseDto()).ToList();
        }
    }
}
