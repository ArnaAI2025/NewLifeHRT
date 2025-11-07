using NewLifeHRT.Application.Services.Interface;
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
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateAddressAsync(AddressDto addressDto, int? userId)
        {
            if (addressDto == null)
                return null;

            var address = new Address(
                addressDto.AddressLine1,
                addressDto.AddressType,
                addressDto.City,
                addressDto.PostalCode,
                addressDto.CountryId,
                addressDto.StateId,
                createdBy: userId?.ToString(),
                createdAt: DateTime.UtcNow,
                isActive : true
            );

            var saved = await _addressRepository.AddAsync(address);
            return new CommonOperationResponseDto<Guid>
            {
                Id = saved.Id,
                Message = "Address Inserted Successfully"
            };
        }
        public async Task<CommonOperationResponseDto<Guid>> UpdateAddressAsync(Guid? existingAddressId, AddressDto dto, int? userId)
        {
            if (dto == null) return null;
            var addr = await _addressRepository.GetByIdAsync(existingAddressId.Value);
            if (addr != null)
            {
                addr.AddressLine1 = dto.AddressLine1;
                addr.AddressType = dto.AddressType;
                addr.City = dto.City;
                addr.PostalCode = dto.PostalCode;
                addr.CountryId = dto.CountryId;
                addr.StateId = dto.StateId;
                addr.UpdatedBy = userId?.ToString();
                addr.UpdatedAt = DateTime.UtcNow;

                await _addressRepository.UpdateAsync(addr);
                return new CommonOperationResponseDto<Guid>
                {
                    Id = addr.Id,
                    Message = "Address updated sucessfully"
                };
            }
            return null;
        }
        public async Task<BulkOperationResponseDto> BulkToggleActiveAsync(IList<Guid> ids, int userId, bool isActive)
        {
            if (ids == null || !ids.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No address IDs provided."
                };
            }

            var addresses = (await _addressRepository.FindAsync(a => ids.Contains(a.Id), noTracking: false)).ToList();

            if (!addresses.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = ids.Count,
                    Message = "No addresses found."
                };
            }

            foreach (var address in addresses)
            {
                address.IsActive = isActive;
                address.UpdatedAt = DateTime.UtcNow;
                address.UpdatedBy = userId.ToString();
            }

            await _addressRepository.BulkUpdateAsync(addresses);
            await _addressRepository.SaveChangesAsync();

            return new BulkOperationResponseDto
            {
                SuccessCount = addresses.Count,
                FailedCount = ids.Count - addresses.Count,
                Message = isActive ? "Addresses activated successfully." : "Addresses deactivated successfully."
            };
        }
        public async Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> addressIds)
        {
            if (addressIds == null || !addressIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No address IDs provided."
                };
            }

            var addresses = (await _addressRepository.FindAsync(a => addressIds.Contains(a.Id), noTracking: false)).ToList();

            if (!addresses.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = addressIds.Count,
                    Message = "No addresses found."
                };
            }

            await _addressRepository.RemoveRangeAsync(addresses);
            await _addressRepository.SaveChangesAsync();

            return new BulkOperationResponseDto
            {
                SuccessCount = addresses.Count,
                FailedCount = addressIds.Count - addresses.Count,
                Message = "Addresses deleted successfully."
            };
        }
        //public async Task<CommonOperationResponseDto<Guid>> SetDefaultAddress(IList<Guid> nonMatchingIds, Guid matchingId, int userId)
        //{
        //    var matching = await _addressRepository.GetByIdAsync(matchingId);
        //    if (matching != null)
        //    {
        //        //matching.IsDefaultAddress = true;
        //        matching.UpdatedAt = DateTime.UtcNow;
        //        matching.UpdatedBy = userId.ToString();
        //        await _addressRepository.UpdateAsync(matching);
        //    }

        //    var nonMatching = (await _addressRepository.FindAsync(a => nonMatchingIds.Contains(a.Id), noTracking: false)).ToList();
        //    foreach (var address in nonMatching)
        //    {
        //        //address.IsDefaultAddress = null;
        //        address.UpdatedAt = DateTime.UtcNow;
        //        address.UpdatedBy = userId.ToString();
        //    }

        //    await _addressRepository.BulkUpdateAsync(nonMatching);
        //    await _addressRepository.SaveChangesAsync();

        //    return new CommonOperationResponseDto<Guid>
        //    {
        //        Id = matchingId,
        //        Message = "Default address updated successfully."
        //    };
        //}

    }
}
