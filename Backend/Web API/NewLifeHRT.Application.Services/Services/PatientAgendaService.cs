using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class PatientAgendaService : IPatientAgendaService
    { 
        public readonly IPatientAgendaRepository _patientAgendaRepository;
        public PatientAgendaService(IPatientAgendaRepository patientAgendaRepository) {
            _patientAgendaRepository = patientAgendaRepository;
        }
        public async Task AddAgendasAsync(IEnumerable<int> agendaIds, Guid patientId, int? userId)
        {
            foreach (var id in agendaIds)
            {
                var entity = new PatientAgenda
                {
                    Id = Guid.NewGuid(),
                    AgendaId = id,
                    PatientId = patientId,
                    IsActive = true,
                    CreatedBy = userId?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                await _patientAgendaRepository.AddAsync(entity);
            }
        }
        public async Task UpdateAgendasAsync(Guid patientId, IEnumerable<int> incomingAgendaIds, IEnumerable<PatientAgenda> existingAgendas, int? userId)
        {
            var agendaSet = incomingAgendaIds?.Distinct().ToHashSet() ?? new HashSet<int>();

            foreach (var dbAgenda in existingAgendas)
            {
                if (agendaSet.Contains(dbAgenda.AgendaId))
                {
                    dbAgenda.IsActive = true;
                    dbAgenda.UpdatedBy = userId?.ToString();
                    dbAgenda.UpdatedAt = DateTime.UtcNow;
                    agendaSet.Remove(dbAgenda.AgendaId);
                }
                else
                {
                    dbAgenda.IsActive = false;
                    dbAgenda.UpdatedBy = userId?.ToString();
                    dbAgenda.UpdatedAt = DateTime.UtcNow;
                }
            }

            foreach (var agendaId in agendaSet)
            {
                var newAgenda = new PatientAgenda
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    AgendaId = agendaId,
                    IsActive = true,
                    CreatedBy = userId?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                await _patientAgendaRepository.AddAsync(newAgenda);
            }
        }

    }
}
