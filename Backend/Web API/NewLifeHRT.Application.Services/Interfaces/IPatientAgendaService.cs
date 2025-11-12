using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IPatientAgendaService
    {
        Task AddAgendasAsync(IEnumerable<int> agendaIds, Guid patientId, int? userId);
        Task UpdateAgendasAsync(Guid patientId, IEnumerable<int> incomingAgendaIds, IEnumerable<PatientAgenda> existingAgendas, int? userId);
    }
}
