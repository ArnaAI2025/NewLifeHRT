using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyShippingMethodMappings
    {
        public static PharmacyShippingMethodResponseDto ToPharmacyShippingMethodResponseDto(this PharmacyShippingMethod shippingMethod)
        {
            return new PharmacyShippingMethodResponseDto
            {
                Id = shippingMethod.Id,
                Name = shippingMethod?.ShippingMethod?.Name,
                Value = shippingMethod.Amount.ToString(),
            };
        }

        public static List<PharmacyShippingMethodResponseDto> ToPharmacyShippingMethodResponseDtoList(this IEnumerable<PharmacyShippingMethod> shippingMethods)
        {
            return shippingMethods.Select(p => p.ToPharmacyShippingMethodResponseDto()).ToList();
        }
    }
}
