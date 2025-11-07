using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IOrderProductsRefillService
    {
        Task<List<OrderProductRefillDetailResponseDto>> GetAllOrderProductRefillAsync();
        Task<int> DeleteOrderProductRefillRecordsAsync(List<Guid> ids);
        Task<OrderProductRefillDetailByIdResponseDto?> GetOrderProductRefillByIdAsync(Guid id);
        Task<bool> UpdateOrderProductRefillDetailAsync(Guid id, UpdateOrderProductRefillDetailRequestDto request, int userId);
    }
}
