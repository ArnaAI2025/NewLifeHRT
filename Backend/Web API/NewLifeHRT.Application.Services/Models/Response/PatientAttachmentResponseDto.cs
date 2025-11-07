using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PatientAttachmentResponseDto
    {
        public Guid? Id { get; set; }
        public Guid? AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string? FileType { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CategoryName { get; set; }
        public string? FileUrl { get; set; }
        public string FileName { get; set; }

    }
}
