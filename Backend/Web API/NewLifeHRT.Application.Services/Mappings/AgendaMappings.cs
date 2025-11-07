using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class AgendaMappings
    {
        public static CommonDropDownResponseDto<int> ToAgendaResponseDto(this Agenda agenda)
        {
            return new CommonDropDownResponseDto<int>
            {
                Id = agenda.Id,
                Value = agenda.AgendaName,
            };
        }
        public static List<CommonDropDownResponseDto<int>> ToAgendaResponseDtoList(this IEnumerable<Agenda> agendas)
        {
            return agendas.Select(p => p.ToAgendaResponseDto()).ToList();
        }
    }
}
