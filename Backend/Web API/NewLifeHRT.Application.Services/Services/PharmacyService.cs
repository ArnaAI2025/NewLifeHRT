using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task ActivatePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (pharmacies == null || !pharmacies.Any())
                throw new Exception("No matching pharmacy found for the provided IDs.");

            foreach (var pharmacy in pharmacies)
            {
                pharmacy.IsActive = true;
                pharmacy.UpdatedBy = userId.ToString();
                pharmacy.UpdatedAt = DateTime.UtcNow;
            }
            await _pharmacyShippingMethodService.SetPharmacyShippingMethodsActivationStatusAsync(pharmacyIds, true, userId);
            await _pharmacyRepository.BulkUpdateAsync(pharmacies);
        }

        public async Task DeactivatePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (pharmacies == null || !pharmacies.Any())
                throw new Exception("No matching pharmacy found for the provided IDs.");

            foreach (var pharmacy in pharmacies)
            {
                pharmacy.IsActive = false;
                pharmacy.UpdatedBy = userId.ToString();
                pharmacy.UpdatedAt = DateTime.UtcNow;
            }
            await _pharmacyShippingMethodService.SetPharmacyShippingMethodsActivationStatusAsync(pharmacyIds, false, userId);
            await _pharmacyRepository.BulkUpdateAsync(pharmacies);
        }

        public async Task<CreatePharmacyResponseDto> CreatePharmacyAsync(PharmacyCreateRequestDto request, int userId)
        {
            var pharmacy = new Pharmacy(
                name: request.Name,
                startDate: request.StartDate,
                endDate: request.EndDate,
                description: request.Description,
                isLab : request.IsLab,
                hasFixedCommission : request.HasFixedCommission,
                commissionPercentage : request.CommissionPercentage,
                createdAt: DateTime.UtcNow,
                createdBy: userId.ToString(),
                currencyId: request.CurrencyId
            );

            var created = await _pharmacyRepository.AddAsync(pharmacy);
            var response = await _pharmacyShippingMethodService.CreatePharmacyShippingMethodAsync(request.ShippingMethods,created.Id, userId);
            return new CreatePharmacyResponseDto { Id = pharmacy.Id };
        }

        public async Task DeletePharmaciesAsync(List<Guid> pharmacyIds, int userId)
        {
            var pharmacies = (await _pharmacyRepository.FindAsync(p => pharmacyIds.Contains(p.Id))).ToList();

            if (pharmacies == null || !pharmacies.Any())
                throw new Exception("No matching pharmacy found for the provided IDs.");

            await _pharmacyRepository.RemoveRangeAsync(pharmacies);
        }

        public async Task<List<PharmacyGetAllResponseDto>> GetAllPharmaciesAsync()
        {
            var pharmacies = await _pharmacyRepository.AllWithIncludeAsync(new[] { "Currency" });
            return pharmacies.ToPharmacyGetAllResponseDtoList();
        }

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

            var pharmacyShippingMethod = await _pharmacyShippingMethodService.UpdatePharmacyShippingMethodAsync(request.ShippingMethods, id, userId);
            var updatedPharmacy = await _pharmacyRepository.UpdateAsync(pharmacy);
            return new CreatePharmacyResponseDto { Id = pharmacy.Id };
        }

        public async Task<List<PharmaciesDropdownResponseDto>> GetAllPharmaciesForDropdownAsync()
        {
            var pharmacies = await _pharmacyRepository.GetAllAsync();
            return pharmacies.ToPharmacyDropdownResponseDtoList();
        }
        public async Task<List<PharmaciesDropdownResponseDto>> GetAllActivePharmaciesForDropdownAsync()
        {
            var pharmacies = await _pharmacyRepository.FindAsync(a => a.IsActive);
            return pharmacies.ToPharmacyDropdownResponseDtoList();
        }
    }
}
