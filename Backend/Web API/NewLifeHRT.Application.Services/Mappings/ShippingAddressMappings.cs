using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ShippingAddressMappings
    {
        public static ShippingAddressResponseDto ToShippingAddressResponseDto(this ShippingAddress shippingAddress)
        {
            return new ShippingAddressResponseDto
            {
                Id = shippingAddress.Id,
                PatientId = shippingAddress.PatientId,
                AddressLine1 = shippingAddress.Address?.AddressLine1,
                AddressType = shippingAddress.Address?.AddressType,
                City = shippingAddress.Address?.City,
                StateId = shippingAddress.Address?.StateId,
                PostalCode = shippingAddress.Address?.PostalCode,
                CountryId = shippingAddress.Address?.CountryId,
                CountryName = shippingAddress.Address?.Country?.Name,
                StateName = shippingAddress.Address?.State?.Name,
                //IsDefaultAddress = shippingAddress.Address?.IsDefaultAddress,
                IsActive = shippingAddress.IsActive,
                AddressId = shippingAddress.AddressId
            };
        }
        public static List<ShippingAddressResponseDto> ToShippingAddressResponseDtoList(this IEnumerable<ShippingAddress> shippingAddresses)
        {
            if (shippingAddresses == null) return new List<ShippingAddressResponseDto>();

            return shippingAddresses.Select(sa => sa.ToShippingAddressResponseDto()).ToList();
        }
    }
}
