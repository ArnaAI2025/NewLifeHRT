using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class PatientAttachmentMappings
    {
        public static PatientAttachmentResponseDto ToPatientAttachmentResponseDto(this PatientAttachment patientAttachment)
        {
            return new PatientAttachmentResponseDto
            {
                Id = patientAttachment.Id,
                AttachmentId = patientAttachment.Attachment.Id,
                AttachmentName = patientAttachment.Attachment.AttachmentName,
                FileType = patientAttachment.Attachment.FileType,
                Extension = patientAttachment.Attachment.Extension,
                CreatedAt = patientAttachment.CreatedAt, 
                CreatedBy = patientAttachment.CreatedBy,
                UpdatedAt = patientAttachment.UpdatedAt,
                UpdatedBy = patientAttachment.UpdatedBy,
                CategoryName = patientAttachment.Attachment.DocumentCategory?.CategoryName
            };
        }
        public static List<PatientAttachmentResponseDto> ToPatientAttachmentResponseDtoList(this IEnumerable<PatientAttachment> patientAttachments)
        {
            return patientAttachments.Select(p => p.ToPatientAttachmentResponseDto()).ToList();
        }
    }
}
