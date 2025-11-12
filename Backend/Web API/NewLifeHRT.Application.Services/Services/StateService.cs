using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync(int countryId)
        {
            var states = await _stateRepository.FindAsync(a => a.IsActive == true && a.CountryId == countryId);
            return states.ToStateResponseDtoList();
        }
    }
}
