using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyDropdownMappings
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
    }
}
