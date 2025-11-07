using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<CommonOperationResponseDto<Guid>> CreateMessageAsync(MessageRequestDto dto, int userId, string createdBy)
        {
                var message = new Message
                {
                    ConversationId = (Guid)dto.ConversationId,
                    UserId = dto.UserId, 
                    TwilioId = dto.TwilioId,
                    IsRead = dto.IsRead,
                    Timestamp = (DateTime)dto.Timestamp,
                    Direction = dto.Direction,
                    IsSent = dto.IsSent,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsActive = true,
                };

                var response = await _messageRepository.AddAsync(message);

                if (Enum.TryParse<MessageDirectionEnum>(dto.Direction, true, out var directionEnum) && directionEnum == MessageDirectionEnum.Outbound)
                {
                    await MarkUnreadInboundMessagesAsReadAsync(message.ConversationId, message.Timestamp, createdBy);
                }

                return new CommonOperationResponseDto<Guid>
                {
                    Id = message.Id,
                    Message = "Message created successfully and unread outbound messages marked as read"
                };
         }


        public async Task<List<MessageResponseDto>> GetUnReadMessagesByCounselorIdAsync(int userId)
        {
            var predicates = new List<Expression<Func<Message, bool>>>
            {
                m => m.UserId == userId,
                m => m.IsRead == false,
                m => m.Direction == MessageDirectionEnum.Inbound.ToString(),
                m => m.Conversation.PatientId != null || m.Conversation.LeadId != null
            };

            var includes = new[]
            { 
                "Conversation.Patient", "Conversation.Lead","MessageContents"
            };

            var messages = await _messageRepository.FindWithIncludeAsync(predicates, includes);
            return messages.ToMessageResponseDtoList();
        }

        public async Task<BulkOperationResponseDto?> UpdateIsReadAsync(BulkOperationRequestDto<Guid> request, int userId)
        {
            var response = new BulkOperationResponseDto();

            if (request == null || request.Ids == null || !request.Ids.Any())
            {
                response.Message = "No IDs provided for update.";
                return response;
            }

            var messages = (await _messageRepository.FindAsync(m => request.Ids.Contains(m.Id))).ToList();

            if (!messages.Any())
            {
                response.Message = "No matching messages found to update.";
                response.FailedCount = request.Ids.Count;
                response.FailedIds = request.Ids.Select(id => id.ToString()).ToList();
                return response;
            }

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.UpdatedBy = userId.ToString();
                message.UpdatedAt = DateTime.UtcNow;
            }

            await _messageRepository.BulkUpdateAsync(messages);

            var updatedIds = messages.Select(m => m.Id.ToString()).ToHashSet();
            var requestedIds = request.Ids.Select(id => id.ToString()).ToHashSet();

            response.SuccessCount = updatedIds.Count;
            response.SuccessIds = updatedIds.ToList();

            response.FailedIds = requestedIds.Except(updatedIds).ToList();
            response.FailedCount = response.FailedIds.Count;

            response.Message = $"IsRead updated for {response.SuccessCount} messages, {response.FailedCount} failed.";

            return response;
        }

        public async Task MarkUnreadInboundMessagesAsReadAsync(Guid conversationId, DateTime messageTimestamp, string updatedBy)
        {
            var unreadInboundMessages = await _messageRepository.FindAsync(m =>
                m.ConversationId == conversationId
                && m.Timestamp < messageTimestamp
                && m.Direction == MessageDirectionEnum.Inbound.ToString()
                && (m.IsRead == false || m.IsRead == null));

            if (unreadInboundMessages == null)
                return;

            foreach (var unread in unreadInboundMessages)
            {
                unread.IsRead = true;
                unread.UpdatedAt = DateTime.UtcNow;
                unread.UpdatedBy = updatedBy;
                await _messageRepository.UpdateAsync(unread);
            }
        }




    }
}
