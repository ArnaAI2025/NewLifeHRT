using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class MedicationTypeMappings
    {
        public static DropDownIntResponseDto ToDropDownIntResponseDto(this MedicationType medicationType)
        {
            return new DropDownIntResponseDto
            {
                Id = medicationType.Id,
                Value = medicationType.MedicationTypeName,
            };
        }
        public static List<DropDownIntResponseDto> ToDropDownIntResponseDtoList(this IEnumerable<MedicationType> medicationTypes)
        {
            return medicationTypes.Select(medicationType  => medicationType.ToDropDownIntResponseDto()).ToList();
        }

    }
}
