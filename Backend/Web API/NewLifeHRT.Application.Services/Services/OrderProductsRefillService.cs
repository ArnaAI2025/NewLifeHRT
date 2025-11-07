using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class OrderProductsRefillService : IOrderProductsRefillService
    {
        private readonly IOrderProductsRefillRepository _orderProductsRefillRepository;

        public OrderProductsRefillService(IOrderProductsRefillRepository orderProductsRefillRepository)
        {
            _orderProductsRefillRepository = orderProductsRefillRepository;
        }

        public async Task<int> DeleteOrderProductRefillRecordsAsync(List<Guid> ids)
        {
            var entitiesToDelete = await _orderProductsRefillRepository
                .FindAsync(x => ids.Contains(x.Id));

            if (entitiesToDelete == null || !entitiesToDelete.Any())
                return 0;

            await _orderProductsRefillRepository.BulkDeleteAsync(entitiesToDelete.ToList());

            return entitiesToDelete.Count();
        }

        public async Task<List<OrderProductRefillDetailResponseDto>> GetAllOrderProductRefillAsync()
        {
            var includes = new[]
            {
                "Order",
                "Order.OrderDetails",
                "ProductPharmacyPriceListItem.Product"
            };

            var entities = await _orderProductsRefillRepository.AllWithIncludeAsync(includes);
            var activeEntities = entities.Where(x => x.IsActive == true);

            return activeEntities.ToOrderProductRefillDetailResponseDtoList();
        }

        public async Task<OrderProductRefillDetailByIdResponseDto?> GetOrderProductRefillByIdAsync(Guid id)
        {
            var includes = new[]
            {
                "ProductPharmacyPriceListItem.Product"
            };

            var entity = await _orderProductsRefillRepository.GetWithIncludeAsync(id, includes);

            if (entity == null || !entity.IsActive)
                return null;

            return entity.ToOrderProductRefillDetailByIdResponseDto();
        }

        public async Task<bool> UpdateOrderProductRefillDetailAsync(Guid id, UpdateOrderProductRefillDetailRequestDto request, int userId)
        {
            var entity = await _orderProductsRefillRepository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.DaysSupply = request.DaysSupply;
            entity.DoseAmount = request.DoseAmount;
            entity.DoseUnit = request.DoseUnit;
            entity.FrequencyPerDay = request.FrequencyPerDay;
            entity.BottleSizeMl = request.BottleSizeMl;
            entity.RefillDate = request.RefillDate;
            entity.Status = request.Status;
            if (!string.IsNullOrWhiteSpace(request.Assumption))
            {
                entity.Assumptions = new List<string> { request.Assumption };
            }
            entity.UpdatedBy = userId.ToString();
            entity.UpdatedAt = DateTime.UtcNow;

            await _orderProductsRefillRepository.UpdateAsync(entity);
            return true;
        }
    }
}
