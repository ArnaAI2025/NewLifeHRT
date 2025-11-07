using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyGetAllMappings
    {
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
    }
}
