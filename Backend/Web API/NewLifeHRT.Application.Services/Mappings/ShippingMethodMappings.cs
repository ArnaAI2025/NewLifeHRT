using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ShippingMethodMappings
    {
        public static CommonDropDownResponseDto<int> ToShippingMethodResponseDto(this ShippingMethod shippingMethod)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = shippingMethod.Id,
                Value = shippingMethod.Name,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToShippingMethodResponseDtoList(this IEnumerable<ShippingMethod> shippingMethods)
        {
            return shippingMethods.Select(p => p.ToShippingMethodResponseDto()).ToList();
        }
    }
}
