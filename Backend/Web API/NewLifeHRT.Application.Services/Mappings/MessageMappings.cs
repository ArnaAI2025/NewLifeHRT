using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class MessageMappings
    {
        public static MessageResponseDto ToMessageResponseDto(this Message entity)
        {
            string name = "Unknown";
            Guid? id = null;
            bool? isPatient = null;
            string phoneNumber = null;
            if (entity.Conversation != null)
            {
                if (entity.Conversation.Patient != null)
                {
                    name = $"{entity.Conversation.Patient.FirstName ?? ""} {entity.Conversation.Patient.LastName ?? ""}".Trim();
                    id = entity.Conversation.PatientId;
                    isPatient = true;
                    phoneNumber = entity.Conversation.Patient?.PhoneNumber;
                }
                    
                else if (entity.Conversation.Lead != null)
                {
                    name = $"{entity.Conversation.Lead.FirstName ?? ""} {entity.Conversation.Lead.LastName ?? ""}".Trim();
                    id = entity.Conversation.LeadId;
                    isPatient = false;
                    phoneNumber = entity.Conversation.Lead?.PhoneNumber;
                }
            }
            if (string.IsNullOrWhiteSpace(name))
                name = "Unknown";

            return new MessageResponseDto
            {
                MessageId = entity.Id,
                CounselorId = entity.UserId,
                CounselorName = entity.User != null ? $"{entity.User.FirstName} {entity.User.LastName}".Trim() : null,
                TwilioId = entity.TwilioId,
                Direction = entity.Direction,
                IsRead = entity.IsRead,
                Id = id,
                Timestamp = entity.Timestamp,
                IsSent = entity.IsSent,
                IsPatient = isPatient,  
                PhoneNumber = phoneNumber,
                Name = name,
                MessageContents = entity.MessageContents != null
                    ? entity.MessageContents.Select(c => new MessageContentResponseDto
                    {
                        ContentId = c.Id,
                        ContentType = c.ContentType,
                        Content = c.Content
                    }).ToList()
                    : new List<MessageContentResponseDto>()
            };
        }

        public static List<MessageResponseDto> ToMessageResponseDtoList(this IEnumerable<Message> entities)
        {
            return entities.Select(e => e.ToMessageResponseDto()).ToList();
        }
    }
}
