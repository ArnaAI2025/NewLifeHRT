using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _agendaRepository;
        public AgendaService(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository ?? throw new ArgumentNullException(nameof(agendaRepository));
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync()
        {
            var activeAgendas = await _agendaRepository.FindAsync(a => a.IsActive == true);
            return AgendaMappings.ToAgendaResponseDtoList(activeAgendas);
        }
    }
}
