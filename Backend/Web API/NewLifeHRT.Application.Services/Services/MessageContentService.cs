using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class MessageContentService : IMessageContentService
    {
        private readonly IMessageContentRepository _messageContentRepository;
        public MessageContentService(IMessageContentRepository messageContentRepository) 
        {
            _messageContentRepository = messageContentRepository;
        }
        public async Task<CommonOperationResponseDto<Guid>> CreateMessageContentAsync(MessageContentRequestDto dto, string userId)
        {
            var content = new MessageContent
            {
                MessageId = (Guid)dto.MessageId,
                ContentType = dto.ContentType!,
                Content = dto.Content!,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                IsActive = true,
            };

            var response = await _messageContentRepository.AddAsync(content);

            return new CommonOperationResponseDto<Guid>
            {
                Id = content.Id,
                Message = "Content added succcessfully"
            };
        }
        public async Task<List<MessageContent>> GetNonTextMessageContentsByPatientIdAsync(Guid patientId)
        {
            var predicates = new List<Expression<Func<MessageContent, bool>>>
            {
                mc => mc.ContentType != "text" && mc.Message.Conversation.PatientId == patientId
            };

            var includes = new[] { "Message", "Message.Conversation" };

            var messageContents = await _messageContentRepository.FindWithIncludeAsync(
                predicates,
                includes,
                noTracking: true
            );

            return messageContents.ToList();
        }


    }
}
