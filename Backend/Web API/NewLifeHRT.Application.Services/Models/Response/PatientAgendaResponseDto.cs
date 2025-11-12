using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PatientAgendaResponseDto
    {
        public Guid Id { get; set; }
        public int AgendaId { get; set; }
        public string Code { get; set; }
    }
}
