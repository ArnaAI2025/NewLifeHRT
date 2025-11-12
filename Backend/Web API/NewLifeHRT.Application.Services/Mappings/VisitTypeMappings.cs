using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class VisitTypeMappings
    {
        public static CommonDropDownResponseDto<int> ToVisitTypeResponseDtoResponseDto(this VisitType visitType)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = visitType.Id,
                Value = visitType.VisitTypeName,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToVisitTypeResponseDtoResponseDtoList(this IEnumerable<VisitType> visitTypes)
        {
            return visitTypes.Select(p => p.ToVisitTypeResponseDtoResponseDto()).ToList();
        }
    }
}
