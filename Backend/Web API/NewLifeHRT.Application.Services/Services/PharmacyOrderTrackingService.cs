using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PharmacyOrderTrackingService : IPharmacyOrderTrackingService
    {
        private readonly IPharmacyOrderTrackingRepository _pharmacyOrderTrackingService;
        public PharmacyOrderTrackingService(IPharmacyOrderTrackingRepository pharmacyOrderTrackingService)
        {
            _pharmacyOrderTrackingService = pharmacyOrderTrackingService;
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateOrderAsync(PharmacyOrderTrackingDto dto, Guid orderId, int userId)
        {
            var existingTracking = await _pharmacyOrderTrackingService
                .GetSingleAsync(t => t.OrderId == orderId && t.IsActive == true);

            if (existingTracking != null)
            {
                existingTracking.CourierServiceName = (CourierServicesEnum)dto.CourierServiceId;
                existingTracking.TrackingNumber = dto.TrackingNumber; 
                existingTracking.UpdatedAt = DateTime.UtcNow;
                existingTracking.UpdatedBy = userId.ToString();

                await _pharmacyOrderTrackingService.UpdateAsync(existingTracking);
                await _pharmacyOrderTrackingService.SaveChangesAsync();

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
                    CourierServiceName = (CourierServicesEnum)dto.CourierServiceId,
                    TrackingNumber = dto.TrackingNumber,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    IsActive = true,
                };

                await _pharmacyOrderTrackingService.AddAsync(request);
                await _pharmacyOrderTrackingService.SaveChangesAsync();

                return new CommonOperationResponseDto<Guid>
                {
                    Id = request.Id,
                    Message = "Order tracking created successfully!"
                };
            }
        }

    }
}
