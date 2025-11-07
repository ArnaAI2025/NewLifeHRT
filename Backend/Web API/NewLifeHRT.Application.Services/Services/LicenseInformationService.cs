using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class LicenseInformationService : ILicenseInformationService
    {
        private readonly ILicenseInformationRepository _licenseInformationRepository;
        public LicenseInformationService(ILicenseInformationRepository licenseInformationRepository)
        {
            _licenseInformationRepository = licenseInformationRepository;
        }
        public async Task<CommonOperationResponseDto<int>> CreateLicenseInformationAsync(LicenseInformationRequestDto[] request, int applicationUserId, int userId)
        {

            try
            {
                var licenseInformation = request.Select(r => new LicenseInformation
                {
                    Id = Guid.NewGuid(),
                    StateId = r.StateId,
                    Number = r.Number,
                    UserId = applicationUserId,
                    Name = "Medical License",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString()
                }).ToList();

                await _licenseInformationRepository.AddRangeAsync(licenseInformation);
                await _licenseInformationRepository.SaveChangesAsync();
                return new CommonOperationResponseDto<int>
                {
                    Id = applicationUserId,
                    Message = "Inserted Successfully"
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<CommonOperationResponseDto<int>> UpdateLicenseInformationAsync( LicenseInformationRequestDto[] request, int applicationUserId,int userId)
        {
            request ??= Array.Empty<LicenseInformationRequestDto>();

            var existing = (await _licenseInformationRepository
                .FindAsync(li => li.UserId == applicationUserId))
                .ToList();

            var existingByState = existing.ToDictionary(e => e.StateId, e => e);
            var requestedByState = request
                .GroupBy(r => r.StateId)
                .Select(g => g.First()) 
                .ToDictionary(r => r.StateId, r => r);

            foreach (var kvp in requestedByState)
            {
                if (existingByState.TryGetValue(kvp.Key, out var entity))
                {
                    var dto = kvp.Value;
                    entity.Number = dto.Number?.Trim();                       
                    entity.UpdatedAt = DateTime.UtcNow;                       
                    entity.UpdatedBy = userId.ToString();                     
                    entity.IsActive = true;                                   
                }
            }

            var newStates = requestedByState.Keys.Except(existingByState.Keys).ToList();
            if (newStates.Count > 0)
            {
                var toAdd = newStates.Select(stateId =>
                {
                    var dto = requestedByState[stateId];
                    return new LicenseInformation
                    {
                        UserId = applicationUserId,
                        StateId = dto.StateId,
                        Name = string.Empty,
                        Number = dto.Number?.Trim(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString()
                    };
                }).ToList();

                if (toAdd.Count > 0)
                {
                    await _licenseInformationRepository.AddRangeAsync(toAdd);
                }
            }

            var removedStates = existingByState.Keys.Except(requestedByState.Keys).ToList();
            if (removedStates.Count > 0)
            {
                var toRemove = removedStates.Select(s => existingByState[s]).ToList();
                if (toRemove.Count > 0)
                {
                    await _licenseInformationRepository.RemoveRangeAsync(toRemove);
                }
            }

            await _licenseInformationRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<int>
            {
                Id = applicationUserId,
                Message = "Updated Successfully"
            };
        }

    }
}
