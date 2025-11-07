using Microsoft.AspNetCore.Http;
using NewLifeHRT.Application.Services.Interface;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IBlobService _blobService;

        public AttachmentService(IAttachmentRepository attachmentRepository, IBlobService blobService)
        {
            _attachmentRepository = attachmentRepository;
            _blobService = blobService;
        }

        public async Task<CommonOperationResponseDto<Guid>> UploadAsync(IFormFile file,int documentCategoryId, Guid patientId, int? userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

           
            // GENERATE UNIQUE FILE NAME
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
            string uniqueFileName = $"{timestamp}_{Path.GetFileNameWithoutExtension(file.FileName)}{Path.GetExtension(file.FileName)}";
            string blobPath = $"{patientId}/{documentCategoryId}/{uniqueFileName}";
            var blobUrl = await _blobService.UploadFileAsync(file, blobPath);

            // Save database record
            var attachment = new Attachment
            {
                AttachmentName = Path.GetFileNameWithoutExtension(file.FileName),
                FileName = uniqueFileName,
                FileType = file.ContentType,
                Extension = Path.GetExtension(file.FileName),
                DocumentCategoryId = documentCategoryId,
                IsActive = true,
                CreatedBy = userId?.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            var response = await _attachmentRepository.AddAsync(attachment);
            await _attachmentRepository.SaveChangesAsync();

            return new CommonOperationResponseDto<Guid>
            {
                Id = response.Id,
                Message = "Attachment saved successfully"
            };
        }
        public async Task<BulkOperationResponseDto> BulkUploadAttachmentAsync(UploadFilesRequestDto requestDto, int userId)
        {
            var results = new List<CommonOperationResponseDto<Guid>>();
            var attachmentsToInsert = new List<Attachment>();

            if (requestDto?.UploadFileItemDto == null )
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                };
            }

            if (!Guid.TryParse(requestDto.Id, out Guid patientId))
            {
                throw new ArgumentException("Invalid patient Id");
            }

            var uploadTasks = requestDto.UploadFileItemDto
                .Where(f => f.File != null && f.File.Length > 0)
                .Select(async fileDto =>
                {
                    try
                    {
                        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
                        string uniqueFileName = $"{timestamp}_{Path.GetFileNameWithoutExtension(fileDto.File.FileName)}{Path.GetExtension(fileDto.File.FileName)}";
                        string categoryFolder = fileDto.DocumentCategoryId?.ToString() ?? "Uncategorized";
                        string blobPath = $"{patientId}/{categoryFolder}/{uniqueFileName}";

                        var blobUrl = await _blobService.UploadFileAsync(fileDto.File, blobPath);

                        // Prepare Attachment entity (not saved yet)
                        return new Attachment
                        {
                            AttachmentName = Path.GetFileNameWithoutExtension(fileDto.File.FileName),
                            FileName = uniqueFileName,
                            FileType = fileDto.File.ContentType,
                            Extension = Path.GetExtension(fileDto.File.FileName),
                            DocumentCategoryId = fileDto.DocumentCategoryId.Value,
                            CreatedBy = userId.ToString(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }).ToList();

            var uploadedAttachments = await Task.WhenAll(uploadTasks);

            attachmentsToInsert =  uploadedAttachments.Where(a => a != null).ToList();

            if (attachmentsToInsert.Count > 0)
            {
                await _attachmentRepository.AddRangeAsync(attachmentsToInsert);
                await _attachmentRepository.SaveChangesAsync();

                foreach (var savedAttachment in attachmentsToInsert)
                {
                    results.Add(new CommonOperationResponseDto<Guid>
                    {
                        Id = savedAttachment.Id,
                        Message = $"Uploaded & saved: {savedAttachment.FileName}"
                    });
                }
            }
            int failedCount = uploadedAttachments.Count(a => a == null);

            return new BulkOperationResponseDto
            {
                SuccessCount = attachmentsToInsert.Count,
                FailedCount = failedCount,
                SuccessIds =  attachmentsToInsert.Select(a => a.Id.ToString()).ToList()
            };
        }



        public async Task<CommonOperationResponseDto<Guid>> ToggleIsActiveStatus(Guid attachmentId, bool isActive, int? userId)
        {

            var attachment = await _attachmentRepository.GetSingleAsync( predicate: p => p.Id == attachmentId,noTracking: false);
            if (attachment == null) return null;
            //string blobPath = $"{CreatedYearMonth}/{patientId}/{documentCategoryId}/{attachment?.FileName}";

            //// Destination path in trash
            //string trashPath = $"trash/{CreatedYearMonth}/{attachment.Id}/{attachment.FileName}";

            // Move (copy + delete from source)
            //await _blobService.CopyFileAsync(blobPath, trashPath);
            //await _blobService.DeleteFileAsync(blobPath);

            // Mark as inactive in DB
            attachment.IsActive = isActive;
            attachment.UpdatedAt = DateTime.UtcNow;
            attachment.UpdatedBy = userId.ToString();
            await _attachmentRepository.UpdateAsync(attachment);
            await _attachmentRepository.SaveChangesAsync();
            return new CommonOperationResponseDto<Guid> { Id = attachment.Id };
        }
        public async Task<BulkOperationResponseDto> BulkToggleDocumentStatusAsync(List<Guid> attachmentIds,int userId,bool isActive)
        {
            if (attachmentIds == null || !attachmentIds.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = 0,
                    Message = "No attachment IDs provided."
                };
            }

            var documentsToUpdate = (await _attachmentRepository.FindAsync(a => attachmentIds.Contains(a.Id), noTracking: false))?.ToList();

            if (documentsToUpdate == null || !documentsToUpdate.Any())
            {
                return new BulkOperationResponseDto
                {
                    SuccessCount = 0,
                    FailedCount = attachmentIds.Count,
                    Message = "No attachments found for the provided IDs."
                };
            }

            foreach (var doc in documentsToUpdate)
            {
                doc.IsActive = isActive;
                doc.UpdatedBy = userId.ToString();
                doc.UpdatedAt = DateTime.UtcNow;
            }

            await _attachmentRepository.BulkUpdateAsync(documentsToUpdate);

            return new BulkOperationResponseDto
            {
                SuccessCount = documentsToUpdate.Count,
                FailedCount = attachmentIds.Count - documentsToUpdate.Count,
                Message = $"{documentsToUpdate.Count} attachment(s) status updated to {(isActive ? "Active" : "Inactive")}."
            };
        }


    }
}
