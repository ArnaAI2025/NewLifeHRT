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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PharmacyShippingMethodService : IPharmacyShippingMethodService
    {
        private readonly IPharmacyShippingMethodRepository _pharmacyShippingMethodRepository;
        public PharmacyShippingMethodService(IPharmacyShippingMethodRepository pharmacyShippingMethodRepository)
        {
            _pharmacyShippingMethodRepository = pharmacyShippingMethodRepository;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreatePharmacyShippingMethodAsync(PharmactShippingMethodRequestDto[] request, Guid pharmacyId, int userId)
        {

            var pharmacyShippingMethods = request.Select(r => new PharmacyShippingMethod
            {
                PharmacyId = pharmacyId,
                Amount = r.Amount,
                CostOfShipping = r.CostOfShipping,
                ShippingMethodId = r.shippingMethodId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            }).ToList();

            await _pharmacyShippingMethodRepository.AddRangeAsync(pharmacyShippingMethods);
            await _pharmacyShippingMethodRepository.SaveChangesAsync();
            return new CommonOperationResponseDto<Guid>
            {
                Id = pharmacyId,
                Message = "Inserted Successfully"
            };
        }
        public async Task<CommonOperationResponseDto<Guid>> UpdatePharmacyShippingMethodAsync(PharmactShippingMethodRequestDto[] request,Guid pharmacyId,int userId)
        {
            try
            {
                var existingMethods = (await _pharmacyShippingMethodRepository
                    .FindAsync(x => x.PharmacyId == pharmacyId))
                    .ToList();

                var requestIds = request
                    .Where(r => r.Id.HasValue && r.Id.Value != Guid.Empty)
                    .Select(r => r.Id!.Value)
                    .ToHashSet();

                foreach (var existing in existingMethods)
                {
                    var req = request.FirstOrDefault(r => r.Id == existing.Id);
                    if (req != null)
                    {
                        existing.ShippingMethodId = req.shippingMethodId;
                        existing.Amount = req.Amount;
                        existing.CostOfShipping = req.CostOfShipping;
                        existing.UpdatedAt = DateTime.UtcNow;
                        existing.UpdatedBy = userId.ToString();
                    }
                }

                var newOnes = request.Where(r => !r.Id.HasValue || r.Id.Value == Guid.Empty)
                    .Select(r => new PharmacyShippingMethod
                    {
                        PharmacyId = pharmacyId,
                        ShippingMethodId = r.shippingMethodId,
                        Amount = r.Amount,
                        CostOfShipping = r.CostOfShipping,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString()
                    }).ToList();

                if (newOnes.Any())
                {
                    await _pharmacyShippingMethodRepository.AddRangeAsync(newOnes);
                }

                var toRemove = existingMethods.Where(e => !requestIds.Contains(e.Id)).ToList();

                if (toRemove.Any())
                {
                    await _pharmacyShippingMethodRepository.RemoveRangeAsync(toRemove);
                }

                await _pharmacyShippingMethodRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid>
                {
                    Id = pharmacyId,
                    Message = "Updated Successfully"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetPharmacyShippingMethodsActivationStatusAsync(List<Guid> pharmacyIds, bool isActive,int userId)
        {
            try
            {
                var methods = (await _pharmacyShippingMethodRepository.FindAsync(p => pharmacyIds.Contains(p.PharmacyId))).ToList();

                if (methods == null || !methods.Any())
                    throw new Exception("No matching PharmacyShippingMethod found for the provided IDs.");

                foreach (var method in methods)
                {
                    method.IsActive = isActive;
                    method.UpdatedBy = userId.ToString();
                    method.UpdatedAt = DateTime.UtcNow;
                }

                await _pharmacyShippingMethodRepository.BulkUpdateAsync(methods);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<PharmacyShippingMethodResponseDto>> GetAllPharmacyShippingMethod(Guid pharmacyId)
        {
            var predicates = new List<Expression<Func<PharmacyShippingMethod, bool>>>
            {
                psm => psm.IsActive == true,
                psm => psm.PharmacyId == pharmacyId
            };

            var includes = new[] { nameof(PharmacyShippingMethod.ShippingMethod) };

            var shippingMethods = await _pharmacyShippingMethodRepository.FindWithIncludeAsync(predicates, includes, noTracking: true);

            return shippingMethods.ToPharmacyShippingMethodResponseDtoList();
        }
        public async Task<decimal?> GetShippingMethodPriceAsync(Guid pharmacyShippingMethodId)
        {
            var item = (await _pharmacyShippingMethodRepository
                .FindAsync(p => p.Id == pharmacyShippingMethodId, noTracking: true))
                .FirstOrDefault();

            return item?.Amount;
        }

    }
}
