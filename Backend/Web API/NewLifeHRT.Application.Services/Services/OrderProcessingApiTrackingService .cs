using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
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
    public class OrderProcessingApiTrackingService : IOrderProcessingApiTrackingService
    {
        private readonly IOrderProcessingApiTrackingRepository _orderProcessingApiTrackingRepository;

        public OrderProcessingApiTrackingService(IOrderProcessingApiTrackingRepository orderProcessingApiTrackingRepository)
        {
            _orderProcessingApiTrackingRepository = orderProcessingApiTrackingRepository;
        }
        public async Task<List<OrderProcessingErrorResponseDto>> GetErrorTrackingsAsync()
        {
            var includes = new[] { "Order", "Order.Pharmacy", "IntegrationType", "Transactions" };

            var errorTrackings = await _orderProcessingApiTrackingRepository.FindWithIncludeAsync(
                new List<Expression<Func<OrderProcessingApiTracking, bool>>>
                {
                    x => x.Order.Status == OrderStatus.LifeFileError
                },
                includes,
                noTracking: true
            );

            return errorTrackings.Select(e => e.ToErrorResponseDto()).ToList();
        }
    }
}
