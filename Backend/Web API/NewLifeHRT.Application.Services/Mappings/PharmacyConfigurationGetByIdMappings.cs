using NewLifeHRT.Common.Helpers;
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
    public static class PharmacyConfigurationGetByIdMappings
    {
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
