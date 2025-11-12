using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;

namespace NewLifeHRT.Application.Services.Services
{
    public class BatchMessageRecipientService : IBatchMessageRecipientService
    {
        private readonly IBatchMessageRecipientRepository _batchMessageRecipientRepository;
        public BatchMessageRecipientService(IBatchMessageRecipientRepository batchMessageRecipientRepository)
        {
            _batchMessageRecipientRepository = batchMessageRecipientRepository;
        }

        public async Task<BulkOperationResponseDto> CreateAsync(Guid batchMessageId, IList<BatchMessageRecipientRequestDto> requestRecipients, int userId)
        {
            var response = new BulkOperationResponseDto();

            if (requestRecipients == null || requestRecipients.Count == 0)
                throw new ArgumentException("Recipient list is empty or null.");

            var failedIds = new List<string>();
            var successIds = new List<string>();

            foreach (var recipientDto in requestRecipients)
            {
                try
                {
                    var recipient = new BatchMessageRecipient
                    {
                        BatchMessageId = batchMessageId,
                        PatientId = recipientDto.PatientId,
                        LeadId = recipientDto.LeadId,
                        Status =  StatusEnum.Draft,
                        ErrorReason = null,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString(),
                        IsActive = true,
                    };

                    await _batchMessageRecipientRepository.AddAsync(recipient);

                    successIds.Add(recipient.Id.ToString());
                    response.SuccessCount++;
                    response.SuccessIds.Add(recipient.Id.ToString());
                }
                catch (Exception ex)
                {
                    failedIds.Add(recipientDto.PatientId?.ToString() ?? "null");
                    response.FailedCount++;
                    response.FailedIds.Add(recipientDto.PatientId?.ToString() ?? "null");
                }
            }
            await _batchMessageRecipientRepository.SaveChangesAsync();
            response.Message = $"BatchRecipient creation completed. Success: {response.SuccessCount}, Failed: {response.FailedCount}.";

            return response;
        }
        public async Task<BulkOperationResponseDto> UpdateAsync(Guid batchMessageId, int status, int userId)
        {
            var response = new BulkOperationResponseDto();
            var existingRecipients = await _batchMessageRecipientRepository.FindAsync(r => r.BatchMessageId == batchMessageId);
            if (!existingRecipients.Any())
                throw new KeyNotFoundException($"BatchMessage with id {batchMessageId} not found or has no recipients.");

            var recipientsList = existingRecipients.ToList();
            int successCount = 0, failCount = 0;
            var failedIds = new List<string>();

            foreach (var recipient in recipientsList)
            {
                try
                {
                    recipient.Status = (StatusEnum)status;
                    recipient.UpdatedBy = userId.ToString();
                    recipient.UpdatedAt = DateTime.UtcNow;
                    await _batchMessageRecipientRepository.UpdateAsync(recipient);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    failedIds.Add(recipient.Id.ToString());
                    // Log ex if needed
                }
            }

            response.SuccessCount = successCount;
            response.FailedCount = failCount;
            response.FailedIds = failedIds;
            response.Message = $"BatchRecipient update completed. Success: {successCount}, Failed: {failCount}.";

            return response;
        }



    }
}
