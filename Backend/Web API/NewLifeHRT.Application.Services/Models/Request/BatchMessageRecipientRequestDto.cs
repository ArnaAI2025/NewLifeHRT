using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class BatchMessageRecipientRequestDto
    {
        public Guid? BatchMessageRecipientId {  get; set; }
        public Guid? PatientId { get; set; }
        public Guid? LeadId { get; set; }
        public int? Status { get; set; } 
        public string? ErrorReason { get; set; }
    }
}
