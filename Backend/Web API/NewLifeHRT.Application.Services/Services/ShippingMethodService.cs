using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly IShippingMethodRepository _shippingMethodRepository;
        public ShippingMethodService(IShippingMethodRepository shippingMethodRepository) {
            _shippingMethodRepository = shippingMethodRepository;   
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllActiveAsync()
        {
            var shippingMethods = await _shippingMethodRepository.FindAsync(a => a.IsActive == true);
            return ShippingMethodMappings.ToShippingMethodResponseDtoList(shippingMethods);
        }
    }
}
