using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class IntegrationTypeMappings
    {
        public static CommonDropDownResponseDto<int> ToIntegrationTypeResponseDto(this IntegrationType integrationType)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = integrationType.Id,
                Value = integrationType.Type,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToIntegrationTypeResponseDtoList(this IEnumerable<IntegrationType> integrationType)
        {
            return integrationType.Select(p => p.ToIntegrationTypeResponseDto()).ToList();
        }
    }
}
