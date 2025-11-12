using Microsoft.AspNetCore.Http;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<CommonOperationResponseDto<Guid>> UploadAsync(IFormFile file, int documentCategory, Guid Id,  int? userId);
        Task<CommonOperationResponseDto<Guid>> ToggleIsActiveStatus(Guid attachmentId, bool status, int? userId);
        Task<BulkOperationResponseDto> BulkUploadAttachmentAsync(UploadFilesRequestDto requestDto, int userId);
        Task<BulkOperationResponseDto> BulkToggleDocumentStatusAsync(List<Guid> attachmentIds, int userId, bool isActive);
    }
}
