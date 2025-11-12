using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PharmacyConfigurationMappings
    {
        public static PharmacyConfigurationGetAllResponseDto ToPharmacyConfigurationGetAllResponseDto(this PharmacyConfigurationEntity pharmacyConfig)
        {
            return new PharmacyConfigurationGetAllResponseDto
            {
                Id = pharmacyConfig.Id,
                PharmacyName = pharmacyConfig.Pharmacy.Name,
                TypeName = pharmacyConfig.IntegrationType.Type,
                Status = pharmacyConfig.IsActive ? "Active" : "Inactive",
                ModifiedOn = pharmacyConfig.UpdatedAt ?? pharmacyConfig.CreatedAt
            };
        }

        public static List<PharmacyConfigurationGetAllResponseDto> ToPharmacyConfigurationGetAllResponseDtoList(this IEnumerable<PharmacyConfigurationEntity> pharmacyConfig)
        {
            return pharmacyConfig.Select(p => p.ToPharmacyConfigurationGetAllResponseDto()).ToList();
        }

        public static PharmacyConfigurationGetByIdResponseDto ToPharmacyConfigurationGetByIdResponseDto(this PharmacyConfigurationEntity pharmacyConfig, string encryptionKey, string encryptionIV)
        {
            return new PharmacyConfigurationGetByIdResponseDto
            {
                PharmacyId = pharmacyConfig.PharmacyId,
                TypeId = pharmacyConfig.TypeId,
                Status = pharmacyConfig.IsActive ? "Active" : "Inactive",
                ConfigData = pharmacyConfig.ConfigurationData.Select(cd => new PharmacyConfigurationKeyValueDto
                {
                    KeyId = cd.KeyId,
                    Value = CryptoHelper.Decrypt(cd.Value, encryptionKey, encryptionIV)
                }).ToList()
            };
        }
    }
}
