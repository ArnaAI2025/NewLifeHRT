using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class BatchMessageResponseDto
    {
        public Guid? BatchMessageId { get; set; }
        public string? Subject { get; set; }
        public string? CreatedByUser { get; set; }
        public string Message { get; set; } = null!;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public bool? IsPatient { get; set; }
        public bool? IsMail { get; set; }
        public bool? IsSms { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Status? Status { get; set; }
        public string? Notes { get; set; }
        public List<BatchMessageRecipientResponseDto> BatchMessageRecipient { get; set; }
    }
}
