using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

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
