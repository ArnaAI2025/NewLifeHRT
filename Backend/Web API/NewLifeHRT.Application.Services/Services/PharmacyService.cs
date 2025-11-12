using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IPharmacyShippingMethodService _pharmacyShippingMethodService;
        public PharmacyService(IPharmacyRepository pharmacyRepository, IPharmacyShippingMethodService pharmacyShippingMethodService)
        {
            _pharmacyRepository = pharmacyRepository;
            _pharmacyShippingMethodService = pharmacyShippingMethodService;
        }

        /// <summary>
        /// Activates multiple pharmacies and their associated shipping methods.
        /// </summary>
        /// <remarks>
        /// This method performs a bulk update operation.
        /// Each pharmacy record is activated (IsActive = true),
        /// and the related shipping methods are also activated using <see cref="_pharmacyShippingMethodService"/>.
        /// </remarks>
        public async Task ActivatePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            if (pharmacyIds == null || pharmacyIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy ID must be provided.", nameof(pharmacyIds));
            }
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (!pharmacies.Any())
            {
                throw new KeyNotFoundException("No matching pharmacy found for the provided IDs.");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var pharmacy in pharmacies)
            {
                pharmacy.IsActive = true;
                pharmacy.UpdatedBy = userId.ToString();
                pharmacy.UpdatedAt = utcNow;
            }

            // Activates associated shipping methods to maintain configuration consistency
            await _pharmacyShippingMethodService.SetPharmacyShippingMethodsActivationStatusAsync(pharmacyIds, true, userId);
            await _pharmacyRepository.BulkUpdateAsync(pharmacies);
        }

        /// <summary>
        /// Deactivates pharmacies and their related shipping methods.
        /// </summary>
        /// <remarks>
        /// Mirrors <see cref="ActivatePharmaciesAsync"/> logic but deactivates entities instead.
        /// </remarks>
        public async Task DeactivatePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            if (pharmacyIds == null || pharmacyIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy ID must be provided.", nameof(pharmacyIds));
            }
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (!pharmacies.Any())
            {
                throw new KeyNotFoundException("No matching pharmacy found for the provided IDs.");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var pharmacy in pharmacies)
            {
                pharmacy.IsActive = false;
                pharmacy.UpdatedBy = userId.ToString();
                pharmacy.UpdatedAt = utcNow;
            }
            // Keeps shipping methods in sync with the parent pharmacy activation state
            await _pharmacyShippingMethodService.SetPharmacyShippingMethodsActivationStatusAsync(pharmacyIds, false, userId);
            await _pharmacyRepository.BulkUpdateAsync(pharmacies);
        }

        /// <summary>
        /// Creates a new pharmacy record along with its default shipping methods.
        /// </summary>
        /// <remarks>
        /// - Performs a compound operation: creates pharmacy and immediately initializes shipping methods.
        /// - Uses <see cref="_pharmacyShippingMethodService"/> to encapsulate logic for shipping method creation.
        /// </remarks>
        public async Task<CreatePharmacyResponseDto> CreatePharmacyAsync(PharmacyCreateRequestDto request, int userId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var pharmacy = new Pharmacy(
                name: request.Name,
                startDate: request.StartDate,
                endDate: request.EndDate,
                description: request.Description,
                isLab: request.IsLab,
                hasFixedCommission: request.HasFixedCommission,
                commissionPercentage: request.CommissionPercentage,
                createdAt: DateTime.UtcNow,
                createdBy: userId.ToString(),
                currencyId: request.CurrencyId
            );

            await _pharmacyRepository.AddAsync(pharmacy);

            var shippingMethods = request.ShippingMethods ?? Array.Empty<PharmactShippingMethodRequestDto>();
            if (shippingMethods.Length > 0)
            {
                await _pharmacyShippingMethodService.CreatePharmacyShippingMethodAsync(shippingMethods, pharmacy.Id, userId);
            }

            return new CreatePharmacyResponseDto { Id = pharmacy.Id };
        }

        /// <summary>
        /// Deletes a set of pharmacies.
        /// </summary>
        /// <remarks>
        /// This method performs a bulk delete but does not cascade-delete related records (e.g., shipping methods).
        /// Such cascading should be handled by database constraints or repository logic.
        /// </remarks>
        public async Task DeletePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            if (pharmacyIds == null || pharmacyIds.Count == 0)
            {
                throw new ArgumentException("At least one pharmacy ID must be provided.", nameof(pharmacyIds));
            }
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (!pharmacies.Any())
            {
                throw new KeyNotFoundException("No matching pharmacy found for the provided IDs.");
            }

            await _pharmacyRepository.RemoveRangeAsync(pharmacies);
        }

        /// <summary>
        /// Retrieves all pharmacies including their currency details.
        /// </summary>
        /// <remarks>
        /// Helper conversion method <c>ToPharmacyGetAllResponseDtoList()</c> maps entity to DTO.
        /// </remarks>
        public async Task<List<PharmacyGetAllResponseDto>> GetAllPharmaciesAsync()
        {
            var pharmacies = await _pharmacyRepository.AllWithIncludeAsync(new[] { "Currency" });
            return pharmacies.ToPharmacyGetAllResponseDtoList();
        }

        /// <summary>
        /// Retrieves a single pharmacy by its ID, including shipping methods and currency.
        /// </summary>
        /// <remarks>
        /// - Uses EF's <c>Include</c> and <c>ThenInclude</c> to eagerly load related entities.
        /// - Converts to DTO via mapping extension.
        /// </remarks>
        public async Task<PharmacyGetResponseDto?> GetPharmacyByIdAsync(Guid id)
        {
            var pharmacy = await _pharmacyRepository.Query()
                .Include(p => p.Currency)
                .Include(p => p.PharmacyShippingMethods)
                    .ThenInclude(psm => psm.ShippingMethod)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pharmacy?.ToPharmacyGetResponseDto();
        }
        public async Task<CreatePharmacyResponseDto> UpdatePharmacyAsync(Guid id, PharmacyCreateRequestDto request, int userId)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var pharmacy = await _pharmacyRepository.GetWithIncludeAsync(id, new[] { "Currency" });

            if (pharmacy == null) throw new Exception("Pharmacy not found");

            pharmacy.Name = request.Name;
            pharmacy.StartDate = request.StartDate;
            pharmacy.EndDate = request.EndDate;
            pharmacy.Description = request.Description;
            pharmacy.IsLab = request.IsLab;
            pharmacy.HasFixedCommission = request.HasFixedCommission;
            pharmacy.CommissionPercentage = request.CommissionPercentage;
            pharmacy.CurrencyId = request.CurrencyId;
            pharmacy.UpdatedAt = DateTime.UtcNow;
            pharmacy.UpdatedBy = userId.ToString();

            var shippingMethods = request.ShippingMethods ?? Array.Empty<PharmactShippingMethodRequestDto>();
            await _pharmacyShippingMethodService.UpdatePharmacyShippingMethodAsync(shippingMethods, id, userId);
            await _pharmacyRepository.UpdateAsync(pharmacy);

            return new CreatePharmacyResponseDto { Id = pharmacy.Id };
        }

        /// <summary>
        /// Retrieves all pharmacies for dropdown binding.
        /// </summary>
        /// <remarks>
        /// The helper <c>ToPharmacyDropdownResponseDtoList()</c> transforms entity data to a minimal DTO set for UI consumption.
        /// </remarks>
        public async Task<List<PharmaciesDropdownResponseDto>> GetAllPharmaciesForDropdownAsync()
        {
            var pharmacies = await _pharmacyRepository.GetAllAsync();
            return pharmacies.ToPharmacyDropdownResponseDtoList();
        }

        /// <summary>
        /// Retrieves only active pharmacies for dropdown selection.
        /// </summary>
        /// <remarks>
        /// Filters by <c>IsActive</c> to exclude inactive or deleted pharmacies.
        /// </remarks>
        public async Task<List<PharmaciesDropdownResponseDto>> GetAllActivePharmaciesForDropdownAsync()
        {
            var pharmacies = await _pharmacyRepository.FindAsync(a => a.IsActive);
            return pharmacies.ToPharmacyDropdownResponseDtoList();
        }
    }
}
