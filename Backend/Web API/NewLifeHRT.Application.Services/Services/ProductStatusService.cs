using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
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
    public class ProductStatusService : IProductStatusService
    {
        private readonly IProductStatusRepository _productStatusRepository;
        public ProductStatusService(IProductStatusRepository productStatusRepository)
        {
            _productStatusRepository = productStatusRepository;
        }
        public async Task<List<ProductStatusResponseDto>> GetAllProductStatusAsync()
        {
            var productStatus = await _productStatusRepository.GetAllAsync();
            return productStatus.ToProductStatusResponseDtoList();
        }
    }
}
