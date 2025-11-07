using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class IntegrationKeyResponseMappings
    {
        public static IntegrationKeyResponseDto ToIntegrationKeyResponseDto(this IntegrationKey integrationKey)
        {
            return new IntegrationKeyResponseDto
            {
                Id = integrationKey.Id,
                IntegrationTypeId = integrationKey.IntegrationTypeId,
                KeyName = integrationKey.KeyName,
                Label = integrationKey.Label
            };
        }
        public static List<IntegrationKeyResponseDto> ToIntegrationKeyResponseDtoList(this IEnumerable<IntegrationKey> integrationKey)
        {
            return integrationKey.Select(p => p.ToIntegrationKeyResponseDto()).ToList();
        }
    }
}
