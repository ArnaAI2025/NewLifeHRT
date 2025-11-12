using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class LifeFileQuantityUnitMappings
    {
        public static LifeFileQuantityUnitResponseDto ToLifeFileQuantityUnitsResponseDto(this LifeFileQuantityUnit quantityUnits)
        {
            return new LifeFileQuantityUnitResponseDto
            {
                Id = quantityUnits.Id,
                Name = quantityUnits.Name
            };
        }

        public static List<LifeFileQuantityUnitResponseDto> ToLifeFileQuantityUnitsResponseDtoList(this IEnumerable<LifeFileQuantityUnit> quantityUnits)
        {
            return quantityUnits.Select(quantityUnits => quantityUnits.ToLifeFileQuantityUnitsResponseDto()).ToList();
        }
    }
}
