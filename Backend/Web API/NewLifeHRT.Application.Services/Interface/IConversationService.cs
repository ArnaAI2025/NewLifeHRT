using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IConversationService
    {
        Task<CommonOperationResponseDto<Guid>> CreateConversation(ConversationRequestDto dto, int userId);
        Task<string> ProcessIncomingSmsAsync(SmsRequestDto request);
        Task<Conversation> GetOrCreateConversationAsync(Guid? patientId, Guid? leadId, int? userId, string? PhoneNumber, string createdBy);
        Task<ConversationResponseDto?> GetConversationOrCreateByPatientOrLeadAsync(Guid? patientId, Guid? leadId, int userId);
        Task<Conversation?> GetConversationByPatientIdAsync(Guid patientId, string[] includes);
        Task<Conversation?> GetConversationByLeadIdAsync(Guid leadId, string[] includes);
    }
}
