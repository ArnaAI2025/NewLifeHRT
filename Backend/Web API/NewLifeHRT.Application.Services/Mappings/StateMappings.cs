using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class StateMappings
    {
        public static CommonDropDownResponseDto<int> ToStateResponseDto(this State state)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = state.Id,
                Value = $"{state.Name} ({state.Abbreviation})"
            };
        }

        public static List<CommonDropDownResponseDto<int>> ToStateResponseDtoList(this IEnumerable<State> states)
        {
            return states.Select(p => p.ToStateResponseDto()).ToList();
        }
    }
}
