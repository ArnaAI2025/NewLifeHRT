using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPharmacyOrderTrackingService
    {
        Task<CommonOperationResponseDto<Guid>> CreateOrderAsync(PharmacyOrderTrackingDto dto, Guid orderId, int userId);
    }
}
