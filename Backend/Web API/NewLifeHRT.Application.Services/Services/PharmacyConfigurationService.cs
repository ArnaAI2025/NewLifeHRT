using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Infrastructure.Helpers;

namespace NewLifeHRT.Application.Services.Services
{
    public class PharmacyConfigurationService : IPharmacyConfigurationService
    {
        private readonly IIntegrationTypeRepository _integrationTypeRepository;
        private readonly IIntegrationKeyRepository _integrationKeyRepository;
        private readonly IPharmacyConfigurationRepository _pharmacyConfigurationRepository;
        private readonly IPharmacyConfigurationDataRepository _pharmacyConfigurationDataRepository;
        private readonly SecuritySettings _settings;
        public PharmacyConfigurationService(IIntegrationTypeRepository integrationTypeRepository, IIntegrationKeyRepository integrationKeyRepository, IPharmacyConfigurationRepository pharmacyConfigurationRepository,
        IPharmacyConfigurationDataRepository pharmacyConfigurationDataRepository, IOptions<SecuritySettings> options)
        {
            _integrationTypeRepository = integrationTypeRepository;
            _integrationKeyRepository = integrationKeyRepository;
            _pharmacyConfigurationRepository = pharmacyConfigurationRepository;
            _pharmacyConfigurationDataRepository = pharmacyConfigurationDataRepository;
            _settings = options.Value;

        }

        /// <summary>
        /// Activates multiple pharmacy configuration records in bulk.
        /// </summary>
        public async Task<BulkOperationResponseDto> ActivatePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds, int userId)
        {
            if (pharmacyConfigurationIds == null || pharmacyConfigurationIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy configuration ID must be provided.", nameof(pharmacyConfigurationIds));
            }
            var response = new BulkOperationResponseDto();

            var configs = await _pharmacyConfigurationRepository.FindAsync(pc => pharmacyConfigurationIds.Contains(pc.Id));
            foreach (var config in configs)
            {
                try
                {
                    config.IsActive = true;
                    config.UpdatedBy = userId.ToString();
                    config.UpdatedAt = DateTime.UtcNow;

                    await _pharmacyConfigurationRepository.UpdateAsync(config);

                    response.SuccessCount++;
                    response.SuccessIds.Add(config.Id.ToString());
                }
                catch
                {
                    response.FailedCount++;
                    response.FailedIds.Add(config.Id.ToString());
                }
            }

            response.Message = $"Activated {response.SuccessCount} configurations, {response.FailedCount} failed.";
            return response;
        }

        /// <summary>
        /// Creates a new pharmacy configuration and its associated key-value configuration data.
        /// </summary>
        public async Task<PharmacyConfigurationResponseDto> CreatePharmacyConfigurationAsync(PharmacyConfigurationRequestDto request, int userId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.ConfigData == null || !request.ConfigData.Any())
            {
                return new PharmacyConfigurationResponseDto
                {
                    Message = "Configuration data is required to create a pharmacy configuration."
                };
            }
            var existingConfig = await _pharmacyConfigurationRepository
            .GetSingleAsync(x => x.PharmacyId == request.PharmacyId);

            if (existingConfig != null)
            {
                return new PharmacyConfigurationResponseDto
                {
                    Message = "Pharmacy already has a configuration. Only one configuration is allowed per pharmacy."
                };
            }

            var newConfig = new PharmacyConfigurationEntity(request.PharmacyId, request.TypeId, userId.ToString(), DateTime.UtcNow);

            await _pharmacyConfigurationRepository.AddAsync(newConfig);

            var configDataList = request.ConfigData.Select(cd => new PharmacyConfigurationData
            {
                Id = Guid.NewGuid(),
                PharmacyConfigurationId = newConfig.Id,
                KeyId = cd.KeyId,
                Value = CryptoHelper.Encrypt(cd.Value, _settings.Key, _settings.IV),
                IsActive =  true,
                CreatedBy = userId.ToString(),
                CreatedAt =  DateTime.UtcNow
            }).ToList();

            await _pharmacyConfigurationDataRepository.AddRangeAsync(configDataList);

            return new PharmacyConfigurationResponseDto
            {
                PharmacyConfigurationId = newConfig.Id,
                PharmacyConfigurationDataIds = configDataList.Select(x => x.Id).ToList(),
                Message = "Pharmacy configuration created successfully."
            };
        }

        /// <summary>
        /// Deactivates multiple pharmacy configurations in bulk.
        /// </summary>
        public async Task<BulkOperationResponseDto> DeactivatePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds, int userId)
        {
            if (pharmacyConfigurationIds == null || pharmacyConfigurationIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy configuration ID must be provided.", nameof(pharmacyConfigurationIds));
            }
            var response = new BulkOperationResponseDto();

            var configs = await _pharmacyConfigurationRepository.FindAsync(pc => pharmacyConfigurationIds.Contains(pc.Id));
            foreach (var config in configs)
            {
                try
                {
                    config.IsActive = false;
                    config.UpdatedBy = userId.ToString();
                    config.UpdatedAt = DateTime.UtcNow;

                    await _pharmacyConfigurationRepository.UpdateAsync(config);

                    response.SuccessCount++;
                    response.SuccessIds.Add(config.Id.ToString());
                }
                catch
                {
                    response.FailedCount++;
                    response.FailedIds.Add(config.Id.ToString());
                }
            }

            response.Message = $"Deactivated {response.SuccessCount} configurations, {response.FailedCount} failed.";
            return response;
        }

        /// <summary>
        /// Deletes pharmacy configurations and all their associated configuration data.
        /// </summary>
        public async Task<BulkOperationResponseDto> DeletePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds)
        {
            if (pharmacyConfigurationIds == null || pharmacyConfigurationIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy configuration ID must be provided.", nameof(pharmacyConfigurationIds));
            }
            var response = new BulkOperationResponseDto();

            foreach (var id in pharmacyConfigurationIds)
            {
                try
                {
                    var config = await _pharmacyConfigurationRepository
                        .GetSingleAsync(pc => pc.Id == id, include: q => q.Include(pc => pc.ConfigurationData));

                    if (config == null)
                    {
                        response.FailedCount++;
                        response.FailedIds.Add(id.ToString());
                        continue;
                    }

                    if (config.ConfigurationData.Any())
                        await _pharmacyConfigurationDataRepository.RemoveRangeAsync(config.ConfigurationData.ToList());

                    await _pharmacyConfigurationRepository.DeleteAsync(config);

                    response.SuccessCount++;
                    response.SuccessIds.Add(id.ToString());
                }
                catch (Exception)
                {
                    response.FailedCount++;
                    response.FailedIds.Add(id.ToString());
                }
            }

            response.Message = $"Deleted {response.SuccessCount} configurations, {response.FailedCount} failed.";
            return response;
        }

        public async Task<List<CommonDropDownResponseDto<int>>> GetActiveIntegrationTypesAsync()
        {
            var integrationTypes = await _integrationTypeRepository.FindAsync(x => x.IsActive, noTracking: true);
            return integrationTypes.ToIntegrationTypeResponseDtoList();
        }

        public async Task<List<PharmacyConfigurationGetAllResponseDto>> GetAllConfigurationsAsync()
        {
            var includes = new[] { "Pharmacy", "IntegrationType" };
            var configs = await _pharmacyConfigurationRepository.AllWithIncludeAsync(includes);

            return configs.ToPharmacyConfigurationGetAllResponseDtoList();
        }

        public async Task<PharmacyConfigurationGetByIdResponseDto?> GetConfigurationByIdAsync(Guid pharmacyConfigurationId)
        {
            var config = await _pharmacyConfigurationRepository.GetSingleAsync(
                pc => pc.Id == pharmacyConfigurationId,
                include: q => q.Include(pc => pc.ConfigurationData)
            );

            if (config == null)
                return null;

            // Decrypts configuration values for display using stored security keys
            return config.ToPharmacyConfigurationGetByIdResponseDto(_settings.Key, _settings.IV);
        }

        public async Task<List<IntegrationKeyResponseDto>> GetIntegrationKeysByTypeIdAsync(int integrationTypeId)
        {
            var keys = await _integrationKeyRepository.FindAsync(x => x.IntegrationTypeId == integrationTypeId && x.IsActive, noTracking: true);
            return keys.ToIntegrationKeyResponseDtoList();
        }

        /// <summary>
        /// Updates an existing pharmacy configuration and synchronizes its configuration data.
        /// </summary>
        public async Task<PharmacyConfigurationResponseDto> UpdatePharmacyConfigurationAsync(Guid pharmacyConfigurationId, PharmacyConfigurationRequestDto request, int userId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var existingConfig = await _pharmacyConfigurationRepository.GetSingleAsync(
                x => x.Id == pharmacyConfigurationId,
                include: q => q.Include(pc => pc.ConfigurationData)
            );

            if (existingConfig == null)
            {
                return new PharmacyConfigurationResponseDto
                {
                    Message = "Pharmacy configuration not found."
                };
            }

            // Update base configuration
            existingConfig.TypeId = request.TypeId;
            existingConfig.UpdatedBy = userId.ToString();
            existingConfig.UpdatedAt = DateTime.UtcNow;
            await _pharmacyConfigurationRepository.UpdateAsync(existingConfig);

            // Identify changes between DB and request
            var existingDataDict = existingConfig.ConfigurationData.ToDictionary(cd => cd.KeyId);

            var requestData = request.ConfigData ?? new List<PharmacyConfigurationKeyValueDto>();
            var requestDataDict = requestData.ToDictionary(cd => cd.KeyId);

            // Keys to delete
            var keysToDelete = existingDataDict.Keys.Except(requestDataDict.Keys).ToList();
            var dataToDelete = existingConfig.ConfigurationData
                .Where(cd => keysToDelete.Contains(cd.KeyId)).ToList();
            if (dataToDelete.Any())
                await _pharmacyConfigurationDataRepository.RemoveRangeAsync(dataToDelete);

            // Keys to update
            var keysToUpdate = existingDataDict.Keys.Intersect(requestDataDict.Keys).ToList();
            foreach (var keyId in keysToUpdate)
            {
                var dbEntity = existingDataDict[keyId];
                var requestEntity = requestDataDict[keyId];
                dbEntity.Value = CryptoHelper.Encrypt(requestEntity.Value, _settings.Key, _settings.IV);
                dbEntity.UpdatedBy = userId.ToString();
                dbEntity.UpdatedAt = DateTime.UtcNow;
            }
            if (keysToUpdate.Any())
                await _pharmacyConfigurationDataRepository.BulkUpdateAsync(keysToUpdate.Select(k => existingDataDict[k]).ToList());

            // Keys to insert
            var keysToInsert = requestDataDict.Keys.Except(existingDataDict.Keys).ToList();
            var dataToInsert = keysToInsert.Select(k => new PharmacyConfigurationData
            {
                Id = Guid.NewGuid(),
                PharmacyConfigurationId = pharmacyConfigurationId,
                KeyId = k,
                Value = CryptoHelper.Encrypt(requestDataDict[k].Value, _settings.Key, _settings.IV),
                IsActive = true,
                CreatedBy = userId.ToString(),
                CreatedAt = DateTime.UtcNow
            }).ToList();
            if (dataToInsert.Any())
                await _pharmacyConfigurationDataRepository.AddRangeAsync(dataToInsert);

            return new PharmacyConfigurationResponseDto
            {
                PharmacyConfigurationId = existingConfig.Id,
                PharmacyConfigurationDataIds = existingConfig.ConfigurationData
                    .Select(x => x.Id)
                    .Union(dataToInsert.Select(x => x.Id))
                    .ToList(),
                Message = "Pharmacy configuration updated successfully."
            };
        }
    }
}
