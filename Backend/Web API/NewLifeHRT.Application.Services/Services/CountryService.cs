using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class CountryService : ICountryService
    {
        public readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync()
        {
            var countries = await _countryRepository.FindAsync(a => a.IsActive == true);
            return countries.ToCountryResponseDtoList();
        }
    }
}
