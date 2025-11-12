using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class MessageContentMappings
    {
        public static PatientAttachmentResponseDto ToPatientAttachmentResponseDto(this MessageContent content)
        {
            return new PatientAttachmentResponseDto
            {
                Id = content.Id,
                AttachmentId = null,
                AttachmentName = "Message", 
                FileType = content.ContentType,
                Extension = System.IO.Path.GetExtension(content.Content),
                CategoryName = "other",
                FileUrl = content.Content,
                CreatedAt = content.CreatedAt,
                CreatedBy = content.CreatedBy
            };
        }

        public static List<PatientAttachmentResponseDto> ToPatientAttachmentResponseDtoList(this IEnumerable<MessageContent> contents)
        {
            return contents.Select(c => c.ToPatientAttachmentResponseDto()).ToList();
        }
    }
}
