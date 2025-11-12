using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyMappings
    {
        public static PharmaciesDropdownResponseDto ToPharmacyDropdownResponseDto(this Pharmacy pharmacy)
        {
            return new PharmaciesDropdownResponseDto
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                IsLab = pharmacy.IsLab,
            };
        }

        public static List<PharmaciesDropdownResponseDto> ToPharmacyDropdownResponseDtoList(this IEnumerable<Pharmacy> pharmacy)
        {
            return pharmacy.Select(pharmacy => pharmacy.ToPharmacyDropdownResponseDto()).ToList();
        }

        public static CommonDropDownResponseDto<Guid> ToCommonDropdownDto(this Pharmacy pharmacy)
        {
            return new CommonDropDownResponseDto<Guid>
            {
                Id = pharmacy.Id,
                Value = pharmacy.Name
            };
        }

        public static List<CommonDropDownResponseDto<Guid>> ToCommonDropdownDtoList(this IEnumerable<Pharmacy> pharmacies)
        {
            return pharmacies.Select(pharmacy => pharmacy.ToCommonDropdownDto()).ToList();
        }

        public static PharmacyGetAllResponseDto ToPharmacyGetAllResponseDto(this Pharmacy pharmacy)
        {
            return new PharmacyGetAllResponseDto
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                StartDate = pharmacy.StartDate,
                EndDate = pharmacy.EndDate,
                CurrencyName = pharmacy.Currency?.CurrencyName ?? "",
                IsActive = pharmacy.IsActive,

            };
        }

        public static List<PharmacyGetAllResponseDto> ToPharmacyGetAllResponseDtoList(this IEnumerable<Pharmacy> pharmacy)
        {
            return pharmacy.Select(pharmacy => pharmacy.ToPharmacyGetAllResponseDto()).ToList();
        }

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
