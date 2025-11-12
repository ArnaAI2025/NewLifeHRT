using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Services
{
    public class PatientAttachmentService : IPatientAttachmentService
    {
        public readonly IPatientAttachmentRepository _patientAttachmentRepository;
        public readonly IAttachmentService _attachmentService;
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;
        private readonly IMessageContentService _messageContentService;

        public PatientAttachmentService(IPatientAttachmentRepository patientAttachmentRepository, IAttachmentService attachmentService, IOptions<AzureBlobStorageSettings> azureBlobStorageSettings, IMessageContentService messageContentService)
        {
            _patientAttachmentRepository = patientAttachmentRepository;
            _attachmentService = attachmentService;
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
            _messageContentService = messageContentService;
        }
        public async Task<CommonOperationResponseDto<Guid>> createAsync(Guid patientId, Guid attachementId, int userId)
        {
            var patientAttachment = new PatientAttachment
            {
                PatientId = patientId,
                AttachmentId = attachementId,
                IsActive = true,
                CreatedBy = userId.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _patientAttachmentRepository.AddAsync(patientAttachment);
            await _patientAttachmentRepository.SaveChangesAsync();
            return new CommonOperationResponseDto<Guid>
            {
                Id = result.Id,
                Message = "patient attachement saved sucessfully"
            };
        }
        public async Task<CommonOperationResponseDto<Guid>> ToggleIsActiveAsync(Guid patientAttachmentId, bool status, int userId)
        {
            // Get the specific attachment (tracked, so noTracking = false)
            var patientAttachment = await _patientAttachmentRepository.GetSingleAsync(
                predicate: p => p.Id == patientAttachmentId,
                noTracking: false
            );

            if (patientAttachment == null)
            {
                return new CommonOperationResponseDto<Guid>
                {
                    Message = "Attachment not found."
                };
            }

            // Update status
            patientAttachment.IsActive = status;
            patientAttachment.UpdatedBy = userId.ToString();
            patientAttachment.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _patientAttachmentRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Message = "Status updated successfully.",
                Id = patientAttachment.Id
            };
        }
        public async Task<BulkOperationResponseDto> BulkUploadAttachmentAsync(UploadFilesRequestDto requestDto, Guid patientId, int userId)
        {
            var result = await _attachmentService.BulkUploadAttachmentAsync(requestDto, userId);

            if (result.SuccessIds != null && result.SuccessIds.Any())
            {
                var patientAttachments = result.SuccessIds.Select(id => new PatientAttachment
                {
                    AttachmentId = Guid.Parse(id),
                    PatientId = patientId,
                    IsActive = true,
                    CreatedBy = userId.ToString(),
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _patientAttachmentRepository.AddRangeAsync(patientAttachments);
                await _patientAttachmentRepository.SaveChangesAsync();
            }

            return result;
        }
        public async Task<List<PatientAttachmentResponseDto>> GetPatientAttachmentsAsync(Guid patientId)
        {
            var predicates = new List<Expression<Func<PatientAttachment, bool>>>
    {
        pa => pa.PatientId == patientId
              && pa.IsActive
              && pa.Attachment.IsActive
              && pa.Attachment.DocumentCategoryId != 4
    };

            var includes = new[] { "Attachment", "Attachment.DocumentCategory" };

            var patientAttachments = await _patientAttachmentRepository.FindWithIncludeAsync(
                predicates,
                includes,
                noTracking: true
            );

            var messageContents = await _messageContentService.GetNonTextMessageContentsByPatientIdAsync(patientId);

            var res = PatientAttachmentMappings.ToPatientAttachmentResponseDtoList(patientAttachments);
            var messageRes = messageContents.ToPatientAttachmentResponseDtoList();

            foreach (var dto in res)
            {
                var pa = patientAttachments.FirstOrDefault(x => x.Id == dto.Id);
                if (pa?.Attachment != null)
                {
                    dto.FileUrl =
                    $"{_azureBlobStorageSettings.ContainerSasUrl}/{pa.PatientId}/{pa.Attachment.DocumentCategoryId}/{pa.Attachment.FileName}?{_azureBlobStorageSettings.SasToken}";
                }
            }
            res.AddRange(messageRes);

            return res;
        }
        public async Task<CommonOperationResponseDto<Guid>> GetPatientAttachmentAsync(Guid patientAttachmentId)
        {          
            var patientAttachment = await _patientAttachmentRepository.GetSingleAsync(
                pa => pa.Id == patientAttachmentId && pa.IsActive,
                include: query => query.Include(pa => pa.Attachment)
            );

            if (patientAttachment == null || patientAttachment.Attachment == null)
                return null;

            var attachment = patientAttachment.Attachment;

            string fileUrl = $"{_azureBlobStorageSettings.ContainerSasUrl}/{patientAttachment.PatientId}/{attachment.DocumentCategoryId}/{attachment.FileName}?{_azureBlobStorageSettings.SasToken}";

            return new CommonOperationResponseDto<Guid>
            {
                Message = fileUrl
            };
        }
        public async Task<BulkOperationResponseDto> BulkToggleDocumentStatusAsync(List<Guid> patientDocumentIds,int userId,bool isActive)
        {
            if (patientDocumentIds == null || !patientDocumentIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No document IDs provided."
                };
            }

            var documentsToUpdate = (await _patientAttachmentRepository
                .FindAsync(pa => patientDocumentIds.Contains(pa.Id), noTracking: false))
                ?.ToList();

            if (documentsToUpdate == null || !documentsToUpdate.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = patientDocumentIds.Count,
                    Message = "No documents found for the provided IDs."
                };
            }

            foreach (var doc in documentsToUpdate)
            {
                doc.IsActive = isActive;
                doc.UpdatedBy = userId.ToString();
                doc.UpdatedAt = DateTime.UtcNow;
            }

            var attachmentIds = documentsToUpdate.Select(d => d.AttachmentId).Where(id => id != Guid.Empty).Distinct().ToList();

            BulkOperationResponseDto attachmentUpdateResult = null;
            if (attachmentIds.Any())
            {
                attachmentUpdateResult = await _attachmentService
                    .BulkToggleDocumentStatusAsync(attachmentIds, userId, isActive);
            }

            await _patientAttachmentRepository.BulkUpdateAsync(documentsToUpdate);

            var finalMessage = $"{documentsToUpdate.Count} patient document(s) status updated to {(isActive ? "Active" : "Inactive")}.";
            if (attachmentUpdateResult != null)
            {
                finalMessage += $" {attachmentUpdateResult.SuccessCount} related attachment(s) updated.";
            }

            return new BulkOperationResponseDto
            {
                SuccessCount = documentsToUpdate.Count,
                FailedCount = patientDocumentIds.Count - documentsToUpdate.Count,
                Message = finalMessage
            };
        }
        //public async Task<List<CommonOperationResponseDto<Guid>>> GetPatientAttachmentsAsync(List<Guid> patientAttachmentIds)
        //{
        //    // Include navigation property: Attachment
        //    var includes = new[] { "Attachment" };

        //    // Build the predicates list (FindWithIncludeAsync accepts multiple predicates)
        //    var predicates = new List<Expression<Func<PatientAttachment, bool>>>
        //        {
        //             pa => patientAttachmentIds.Contains(pa.Id) && pa.IsActive
        //        };

        //    var patientAttachments = await _patientAttachmentRepository.FindWithIncludeAsync(
        //        predicates,includes);

        //    var result = new List<CommonOperationResponseDto<Guid>>();

        //    foreach (var patientAttachment in patientAttachments)
        //    {
        //        if (patientAttachment?.Attachment != null)
        //        {
        //            var attachment = patientAttachment.Attachment;

        //            string fileUrl = $"{containerBase}/{patientAttachment.PatientId}/{attachment.DocumentCategoryId}/{attachment.FileName}?{sasToken}";

        //            result.Add(new CommonOperationResponseDto<Guid>
        //            {
        //                Id = patientAttachment.Id,
        //                Message = fileUrl
        //            });
        //        }
        //    }

        //    return result;
        //}





    }
}
