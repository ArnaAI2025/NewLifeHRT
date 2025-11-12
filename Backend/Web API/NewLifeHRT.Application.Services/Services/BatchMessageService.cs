using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Services
{
    public class BatchMessageService : IBatchMessageService
    {
        private readonly IBatchMessageRepository _batchMessageRepository;
        private readonly IBatchMessageRecipientService _batchMessageRecipientService;
        public BatchMessageService(IBatchMessageRepository batchMessageRepository, IBatchMessageRecipientService batchMessageRecipientService)
        {
            _batchMessageRepository = batchMessageRepository;
            _batchMessageRecipientService = batchMessageRecipientService;
        }
        public async Task<List<BatchMessageResponseDto>?> GetAllAsync()
        {
            var predicates = new List<Expression<Func<BatchMessage, bool>>>
            {
                x => x.IsActive == true
            };

            var response = await _batchMessageRepository.FindWithIncludeAsync(
                predicates,
                new string[]
                {
            nameof(BatchMessage.BatchMessageRecipients),
            nameof(BatchMessage.MessageCreatedBy)
                }
            );

            if (response == null || !response.Any())
                return null;

            return response.ToBatchMessageResponseDtoList();
        }




        public async Task<BatchMessageResponseDto?> GetByIdAsync(Guid id)
        {
            var includes = new string[]
            { nameof(BatchMessage.BatchMessageRecipients),
                    "BatchMessageRecipients.Lead",
                    "BatchMessageRecipients.Patient"
            };

           var response = await _batchMessageRepository.GetWithIncludeAsync(id, includes);

            return response?.ToBatchMessageResponseDto();
        }

        public async Task<CommonOperationResponseDto<Guid>> CreateAsync(BatchMessageRequestDto request, int userId)
        {
            var batchMessage = new BatchMessage
            {
                Subject = request.Subject,
                Message = request.Message,
                IsMail = request.IsMail != null ? (bool)request.IsMail : false,
                IsSms = request.IsSms != null ? (bool)request.IsSms : false,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                StatusChangedByUserId = request.ApprovedByUserId,
                //ApprovedAt = request.ApprovedAt,
                Status = StatusEnum.Draft,
                //Notes = request.Notes,
                IsActive = true,
            };

            var batchMessageResponse = await _batchMessageRepository.AddAsync(batchMessage);

            if (request.BatchMessageRecipients != null && request.BatchMessageRecipients.Any())
            {
                await _batchMessageRecipientService.CreateAsync(batchMessageResponse.Id, request.BatchMessageRecipients, userId);
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = batchMessageResponse.Id,

            };
        }

        public async Task<CommonOperationResponseDto<Guid>> UpdateAsync(Guid id, BatchMessageRequestDto request, int status, int userId)
        {
            var entity = await _batchMessageRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"BatchMessage with id {id} not found");

            entity.Subject = request.Subject;
            entity.Message = request.Message;
            entity.StatusChangedByUserId = request.ApprovedByUserId;
            //entity.ApprovedAt = request.ApprovedAt;
            entity.Status = (StatusEnum)status;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId.ToString();
            await _batchMessageRepository.UpdateAsync(entity);

            if (request.BatchMessageRecipients != null)
            {
                await _batchMessageRecipientService.UpdateAsync(entity.Id, status, userId);
            }

            return new CommonOperationResponseDto<Guid>
            {
                Id = entity.Id,
            };
        }
        public async Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> batchMessages)
        {
            if (batchMessages == null || !batchMessages.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No valid messages IDs provided."
                };
            }

            var proposalsToDelete = (await _batchMessageRepository.FindAsync(p => batchMessages.Contains(p.Id), noTracking: false)).ToList();
            if (!proposalsToDelete.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = batchMessages.Count,
                    Message = "No messages found for the provided IDs."
                };
            }
            await _batchMessageRepository.RemoveRangeAsync(proposalsToDelete);
            await _batchMessageRepository.SaveChangesAsync();

            var successCount = proposalsToDelete.Count;
            var failedCount = batchMessages.Count - successCount;

            return new BulkOperationResponseDto
            {
                SuccessCount = successCount,
                FailedCount = failedCount,
                Message = $"{successCount} messages(s) deleted successfully."
            };
        }
    }
}
