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
    public class LifeFileQuantityUnitsService : ILifeFileQuantityUnitsService
    {
        private readonly ILifeFileQuantityUnitsRepository _lifeFileQuantityUnitsRepository;
        public LifeFileQuantityUnitsService(ILifeFileQuantityUnitsRepository lifeFileQuantityUnitsRepository)
        {
            _lifeFileQuantityUnitsRepository = lifeFileQuantityUnitsRepository;
        }

        public async Task<List<LifeFileQuantityUnitsResponseDto>> GetAllLifeFileQuantityUnitsAsync()
        {
            var lifeFileQuantityUnits = await _lifeFileQuantityUnitsRepository.GetAllAsync();
            return lifeFileQuantityUnits.ToLifeFileQuantityUnitsResponseDtoList();
        }
    }
}
