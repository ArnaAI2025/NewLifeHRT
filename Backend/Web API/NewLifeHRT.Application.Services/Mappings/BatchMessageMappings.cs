using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class BatchMessageMappings
    {
        public static BatchMessageResponseDto ToBatchMessageResponseDto(this BatchMessage entity)
        {
            var firstRecipient = entity.BatchMessageRecipients?.FirstOrDefault();
            bool isPatient = firstRecipient != null ? firstRecipient.PatientId != null : false;

            return new BatchMessageResponseDto
            {
                BatchMessageId = entity.Id,
                Subject = entity.Subject,
                Message = entity.Message,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedByUser = entity.MessageCreatedBy != null
                    ? $"{entity.MessageCreatedBy.FirstName} {entity.MessageCreatedBy.LastName}".Trim()
                    : null,
                CreatedAt = entity.CreatedAt,
                ApprovedByUserId = entity.StatusChangedByUserId,
                //ApprovedAt = entity.ApprovedAt,
                IsPatient = isPatient,
                Status = entity.Status,
                //Notes = entity.Notes,
                BatchMessageRecipient = entity.BatchMessageRecipients?.Select(r => new BatchMessageRecipientResponseDto
                {
                    BatchMessageRecipientId = r.Id,
                    PatientId = r.PatientId,
                    LeadId = r.LeadId,
                    Name = r.Lead != null
                        ? $"{r.Lead.FirstName} {r.Lead.LastName}".Trim()
                        : r.Patient != null
                            ? $"{r.Patient.FirstName} {r.Patient.LastName}".Trim()
                            : null,
                    Status = r.Status
                }).ToList() ?? new List<BatchMessageRecipientResponseDto>()
            };
        }


        public static List<BatchMessageResponseDto> ToBatchMessageResponseDtoList(this IEnumerable<BatchMessage> entities)
        {
            return entities.Select(e => e.ToBatchMessageResponseDto()).ToList();
        }
    }
}
