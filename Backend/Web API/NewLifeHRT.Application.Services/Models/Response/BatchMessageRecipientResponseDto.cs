using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class BatchMessageRecipientResponseDto
    {
        public Guid? BatchMessageRecipientId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? LeadId { get; set; }
        public string? Name { get; set; }    
        public StatusEnum? Status { get; set; }
    }
}
