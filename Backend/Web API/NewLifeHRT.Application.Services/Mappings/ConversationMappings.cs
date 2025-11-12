using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class ConversationMappings
    {
        public static ConversationResponseDto ToConversationResponseDto(this Conversation conversation, int? currentCounselorId, string currentCounselorName)
        {
            if (conversation == null)
                return null;

            string name;
            string phoneNumber;

            if (conversation.PatientId.HasValue && conversation.Patient != null)
            {
                phoneNumber = conversation.Patient.PhoneNumber;
                name = $"{conversation.Patient.FirstName ?? ""} {conversation.Patient.LastName ?? ""}".Trim();
            }
            else if (conversation.Lead != null)
            {
                phoneNumber = conversation.Lead.PhoneNumber;
                name = $"{conversation.Lead.FirstName ?? ""} {conversation.Lead.LastName ?? ""}".Trim();
            }
            else
            {
                phoneNumber = null;
                name = "Unknown";
            }

            var messagesDto = conversation.Messages != null
                ? conversation.Messages.OrderBy(m => m.Timestamp)
                    .Select(m => m.ToMessageResponseDto())
                    .ToList()
                : new List<MessageResponseDto>();

            return new ConversationResponseDto
            {
                ConversationId = conversation.Id,
                PatientId = conversation.PatientId,
                LeadId = conversation.LeadId,
                PhoneNumber = phoneNumber,
                Name = name,
                CurrentCounselorId = currentCounselorId,
                CurrentCounselorName = currentCounselorName,
                Messages = messagesDto
            };
        }

        public static List<ConversationResponseDto> ToConversationResponseDtoList(this IEnumerable<Conversation> conversations, int? currentCounselorId, string currentCounselorName)
        {
            return conversations.Select(c => c.ToConversationResponseDto(currentCounselorId, currentCounselorName)).ToList();
        }
    }
}
