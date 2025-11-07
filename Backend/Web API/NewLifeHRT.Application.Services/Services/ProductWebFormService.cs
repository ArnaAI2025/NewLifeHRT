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
    public class ProductWebFormService : IProductWebFormService
    {
        private readonly IProductWebFormRepository _productWebFormRepository;
        public ProductWebFormService(IProductWebFormRepository productWebFormRepository)
        {
            _productWebFormRepository = productWebFormRepository;
        }

        public async Task<List<ProductWebFormResponseDto>> GetAllProductWebFormsAsync()
        {
            var productWebForms = await _productWebFormRepository.GetAllAsync();
            return productWebForms.ToProductWebFormsResponseDtoList();
        }
    }
}
