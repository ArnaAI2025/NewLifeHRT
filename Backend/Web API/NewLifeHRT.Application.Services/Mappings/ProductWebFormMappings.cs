using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ProductWebFormMappings
    {
        public static ProductWebFormResponseDto ToProductWebFormsResponseDto(this ProductWebForm type)
        {
            return new ProductWebFormResponseDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        public static List<ProductWebFormResponseDto> ToProductWebFormsResponseDtoList(this IEnumerable<ProductWebForm> webFormss)
        {
            return webFormss.Select(t => t.ToProductWebFormsResponseDto()).ToList();
        }
    }
}
