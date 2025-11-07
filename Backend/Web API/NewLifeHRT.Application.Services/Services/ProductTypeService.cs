using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Application.Services.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _productTypeRepository;

        public ProductTypeService(IProductTypeRepository productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }

        public async Task<List<ProductTypeResponseDto>> GetAllProductTypesAsync()
        {
            var productTypes = await _productTypeRepository.GetAllAsync();
            return productTypes.ToProductTypeResponseDtoList();
        }
    }
}
