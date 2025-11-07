using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IAgendaService
    {
        Task<List<CommonDropDownResponseDto<int>>> GetAllAsync();
    }
}
