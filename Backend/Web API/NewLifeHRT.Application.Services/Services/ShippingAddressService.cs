using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly IShippingAddressRepository _shippingAddressRepository;
        private readonly IAddressService _addressService;

        public ShippingAddressService(
            IShippingAddressRepository shippingAddressRepository,
            IAddressService addressService)
        {
            _shippingAddressRepository = shippingAddressRepository;
            _addressService = addressService;
        }

        #region Get Methods
        public async Task<List<ShippingAddressResponseDto>> GetAllAsync(Guid patientId)
        {
            var filters = new List<Expression<Func<ShippingAddress, bool>>> { sa => sa.PatientId == patientId };
            var includes = new[] { "Address.Country", "Address.State" };

            var shippingAddresses = await _shippingAddressRepository.FindWithIncludeAsync(filters, includes);

            return shippingAddresses.ToShippingAddressResponseDtoList();
        }


        public async Task<ShippingAddressResponseDto> GetAllByIdAsync(Guid id)
        {
            var shippingAddress = await _shippingAddressRepository.GetSingleAsync(
                sa => sa.Id == id,
                include: query => query.Include(sa => sa.Address)
            );
            if (shippingAddress == null) return null;
            return shippingAddress?.ToShippingAddressResponseDto();
        }
        #endregion

        #region Create
        public async Task<CommonOperationResponseDto<Guid>> CreateAsync(ShippingAddressRequestDto requestDto, int userId, Guid? addressId, bool? setIsDefaultAddress)
        {
            Guid finalAddressId;

            if (addressId.HasValue && addressId.Value != Guid.Empty)
            {
                finalAddressId = addressId.Value;
            }
            else
            {
                if (requestDto?.Address == null)
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Id = Guid.Empty,
                        Message = "Invalid address data"
                    };
                }

                var addressResponse = await _addressService.CreateAddressAsync(requestDto.Address, userId);
                if (addressResponse == null || addressResponse.Id == Guid.Empty)
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Id = Guid.Empty,
                        Message = "Failed to create address"
                    };
                }

                finalAddressId = addressResponse.Id;
            }

            var shippingAddress = new ShippingAddress
            {
                AddressId = finalAddressId,
                PatientId = requestDto.PatientId,
                IsDefaultAddress = setIsDefaultAddress != null && setIsDefaultAddress == true ? true : false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _shippingAddressRepository.AddAsync(shippingAddress);

            return new CommonOperationResponseDto<Guid>
            {
                Id = shippingAddress.Id,
                Message = "Shipping address created successfully!"
            };
        }


        #endregion

        #region Update
        public async Task<CommonOperationResponseDto<Guid>> UpdateAsync(ShippingAddressRequestDto requestDto, int userId)
        {
            var existingShippingAddress = await _shippingAddressRepository.GetByIdAsync(requestDto.Id);
            if (existingShippingAddress == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = "Shipping address not found"
                };
            }

            if (requestDto.Address != null && existingShippingAddress.AddressId != Guid.Empty)
            {
                await _addressService.UpdateAddressAsync(existingShippingAddress.AddressId, requestDto.Address, userId);
            }

            existingShippingAddress.UpdatedAt = DateTime.UtcNow;
            existingShippingAddress.UpdatedBy = userId.ToString();

            await _shippingAddressRepository.UpdateAsync(existingShippingAddress);

            return new CommonOperationResponseDto<Guid>
            {
                Id = existingShippingAddress.Id,
                Message = "Shipping address updated successfully!"
            };
        }
        public async Task<CommonOperationResponseDto<Guid>> SetDefaultAsync(Guid patientId, Guid shippingAddressId, int userId)
        {
            var shippingAddresses = await _shippingAddressRepository
                .FindAsync(sa => sa.PatientId == patientId);

            if (shippingAddresses == null || !shippingAddresses.Any())
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Id = Guid.Empty,
                    Message = "No shipping addresses found for this patient."
                };
            }

            Guid updatedId = Guid.Empty;

            foreach (var sa in shippingAddresses)
            {
                if (sa.Id == shippingAddressId)
                {
                    sa.IsDefaultAddress = true;
                    updatedId = sa.Id;
                }
                else
                {
                    sa.IsDefaultAddress = false;
                }

                sa.UpdatedAt = DateTime.UtcNow;
                sa.UpdatedBy = userId.ToString();
            }

            await _shippingAddressRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = updatedId,
                Message = updatedId == Guid.Empty
                    ? "Provided address not found in patient's shipping addresses."
                    : "Default shipping address updated successfully."
            };
        }

        #endregion

        #region Bulk Activate / Deactivate
        public async Task<BulkOperationResponseDto> BulkToggleActiveAsync(IList<Guid> ids, int userId, bool isActive)
        {
            if (ids == null || !ids.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No shipping address IDs provided."
                };
            }

            var shippingAddresses = (await _shippingAddressRepository
                .FindAsync(sa => ids.Contains(sa.Id), noTracking: false))
                .ToList();

            if (!shippingAddresses.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = ids.Count,
                    Message = "No shipping addresses found."
                };
            }

            var addressIds = shippingAddresses
                .Where(sa => sa.AddressId != Guid.Empty)
                .Select(sa => sa.AddressId)
                .Distinct()
                .ToList();

            if (addressIds.Any())
            {
                await _addressService.BulkToggleActiveAsync(addressIds, userId, isActive);
            }

            foreach (var sa in shippingAddresses)
            {
                sa.IsActive = isActive;
                sa.UpdatedAt = DateTime.UtcNow;
                sa.UpdatedBy = userId.ToString();
            }

            await _shippingAddressRepository.BulkUpdateAsync(shippingAddresses);
            await _shippingAddressRepository.SaveChangesAsync();

            return new BulkOperationResponseDto
            {
                SuccessCount = shippingAddresses.Count,
                FailedCount = ids.Count - shippingAddresses.Count,
                Message = isActive ? "Shipping addresses and linked addresses activated successfully." : "Shipping addresses and linked addresses deactivated successfully."
            };
        }


        #endregion

        #region Bulk Delete
        public async Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return new BulkOperationResponseDto
                    {
                        SuccessCount = 0,
                        FailedCount = 0,
                        Message = "No shipping address IDs provided."
                    };
                }

                var shippingAddresses = (await _shippingAddressRepository
                    .FindAsync(sa => ids.Contains(sa.Id), noTracking: true))
                    .ToList();

                if (!shippingAddresses.Any())
                {
                    return new BulkOperationResponseDto
                    {
                        SuccessCount = 0,
                        FailedCount = ids.Count,
                        Message = "No shipping addresses found."
                    };
                }

                var addressIds = shippingAddresses.Where(sa => sa.AddressId != Guid.Empty).Select(sa => sa.AddressId).Distinct().ToList();

                if (addressIds.Any())
                {
                    await _addressService.BulkDeleteAsync(addressIds);
                }
                return new BulkOperationResponseDto
                {
                    SuccessCount = shippingAddresses.Count,
                    FailedCount = ids.Count - shippingAddresses.Count,
                    Message = "Shipping addresses and linked addresses deleted successfully."
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
