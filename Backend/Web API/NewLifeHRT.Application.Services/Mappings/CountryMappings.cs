using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CountryMappings
    {
        public static CommonDropDownResponseDto<int> ToCountryResponseDto(this Country country)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = country.Id,
                Value = country.Name,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToCountryResponseDtoList(this IEnumerable<Country> countries)
        {
            return countries.Select(p => p.ToCountryResponseDto()).ToList();
        }
    }
}
