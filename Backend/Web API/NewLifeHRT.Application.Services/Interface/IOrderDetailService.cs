using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IOrderDetailService
    {

        Task<BulkOperationResponseDto> CreateOrderDetailAsync(Guid orderId, IList<OrderDetailRequestDto> details, int userId);
        Task<BulkOperationResponseDto> UpdateOrderDetailAsync(Guid orderId, IList<OrderDetailRequestDto> details, int userId);
    }
}
