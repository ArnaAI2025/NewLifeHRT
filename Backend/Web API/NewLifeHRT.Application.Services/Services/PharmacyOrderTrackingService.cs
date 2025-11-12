using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PharmacyOrderTrackingService : IPharmacyOrderTrackingService
    {
        private readonly IPharmacyOrderTrackingRepository _pharmacyOrderTrackingRepository;

        public PharmacyOrderTrackingService(IPharmacyOrderTrackingRepository pharmacyOrderTrackingRepository)
        {
            _pharmacyOrderTrackingRepository = pharmacyOrderTrackingRepository;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateOrderAsync(PharmacyOrderTrackingDto dto, Guid orderId, int userId)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (!dto.CourierServiceId.HasValue && string.IsNullOrWhiteSpace(dto.TrackingNumber))
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "A courier service or tracking number must be provided to record shipping details."
                };
            }

            CourierServiceEnum? courierService = null;
            if (dto.CourierServiceId.HasValue)
            {
                if (!Enum.IsDefined(typeof(CourierServiceEnum), dto.CourierServiceId.Value))
                {
                    return new CommonOperationResponseDto<Guid>
                    {
                        Message = "Invalid courier service identifier provided."
                    };
                }

                courierService = (CourierServiceEnum)dto.CourierServiceId.Value;
            }

            var trackingNumber = string.IsNullOrWhiteSpace(dto.TrackingNumber)
                ? null
                : dto.TrackingNumber.Trim();

            var existingTracking = await _pharmacyOrderTrackingRepository
                .GetSingleAsync(t => t.OrderId == orderId && t.IsActive);

            if (existingTracking != null)
            {
                existingTracking.CourierServiceName = courierService;
                existingTracking.TrackingNumber = trackingNumber;
                existingTracking.UpdatedAt = DateTime.UtcNow;
                existingTracking.UpdatedBy = userId.ToString();

                await _pharmacyOrderTrackingRepository.UpdateAsync(existingTracking);
                await _pharmacyOrderTrackingRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid>
                {
                    Id = existingTracking.Id,
                    Message = "Order tracking updated successfully!"
                };
            }
            else
            {
                var request = new PharmacyOrderTracking
                {
                    OrderId = orderId,
                    CourierServiceName = courierService,
                    TrackingNumber = trackingNumber,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    IsActive = true,
                };

                await _pharmacyOrderTrackingRepository.AddAsync(request);
                await _pharmacyOrderTrackingRepository.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid>
                {
                    Id = request.Id,
                    Message = "Order tracking created successfully!"
                };
            }
        }
    }
}
