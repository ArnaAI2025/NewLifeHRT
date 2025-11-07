using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyGetMappings
    {
        public static PharmacyGetResponseDto ToPharmacyGetResponseDto(this Pharmacy pharmacy)
        {
            return new PharmacyGetResponseDto
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                StartDate = pharmacy.StartDate,
                EndDate = pharmacy.EndDate,
                Description = pharmacy.Description,
                CurrencyId = pharmacy.CurrencyId,
                IsLab = pharmacy.IsLab,
                HasFixedCommission = pharmacy.HasFixedCommission,
                CommissionPercentage = pharmacy.CommissionPercentage,
                IsActive = pharmacy.IsActive,

                ShippingMethods = pharmacy.PharmacyShippingMethods?
                    .Select(sm => new PharmacyShippingResponseDto
                    {
                        Id = sm.Id, 
                        ShippingMethodId = sm.ShippingMethodId,
                        Value = sm.ShippingMethod?.Name ?? string.Empty,
                        Amount = sm.Amount,
                        CostOfShipping = sm.CostOfShipping
                    })
                    .ToArray() ?? Array.Empty<PharmacyShippingResponseDto>() 
            };
        }



        public static List<PharmacyGetResponseDto> ToPharmacyGetResponseDtoList(this IEnumerable<Pharmacy> pharmacy)
        {
            return pharmacy.Select(pharmacy => pharmacy.ToPharmacyGetResponseDto()).ToList();
        }
    }
}
