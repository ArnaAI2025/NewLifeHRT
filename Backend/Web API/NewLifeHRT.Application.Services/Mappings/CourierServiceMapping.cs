using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class CourierServiceMapping
    {
        public static CommonDropDownResponseDto<int> ToCourierServiceResponseDto(this CourierService courierService)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = courierService.Id,
                Value = courierService.Name,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToCourierServiceResponseDtoList(this IEnumerable<CourierService> courierServices)
        {
            return courierServices.Select(p => p.ToCourierServiceResponseDto()).ToList();
        }
    }
}
