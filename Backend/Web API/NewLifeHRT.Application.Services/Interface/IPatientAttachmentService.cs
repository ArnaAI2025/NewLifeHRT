using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPatientAttachmentService
    {
        Task<CommonOperationResponseDto<Guid>> createAsync(Guid patientId, Guid attachementId, int userId);
        Task<CommonOperationResponseDto<Guid>> ToggleIsActiveAsync(Guid patientAttachmentId, bool status, int userId);
        Task<BulkOperationResponseDto> BulkUploadAttachmentAsync(UploadFilesRequestDto requestDto, Guid patientId, int userId);
        Task<List<PatientAttachmentResponseDto>> GetPatientAttachmentsAsync(Guid patientId);
        Task<CommonOperationResponseDto<Guid>> GetPatientAttachmentAsync(Guid patientAttachmentId);
        Task<BulkOperationResponseDto> BulkToggleDocumentStatusAsync(List<Guid> patientDocumentIds, int userId, bool isActive);
        //Task<List<CommonOperationResponseDto<Guid>>> GetPatientAttachmentsAsync(List<Guid> patientAttachmentIds);

    }
}
