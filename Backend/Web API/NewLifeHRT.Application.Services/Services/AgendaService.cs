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
    public class AgendaService : IAgendaService
    {
        public IAgendaRepository _agendaRepository;
        public AgendaService(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }
        public async Task<List<CommonDropDownResponseDto<int>>> GetAllAsync()
        {
            var activeAgendas = await _agendaRepository.FindAsync(a => a.IsActive == true);
            return AgendaMappings.ToAgendaResponseDtoList(activeAgendas);
        }

    }
}
