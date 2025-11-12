using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ConversationResponseDto
    {
        public Guid ConversationId { get; set; }
        public Guid? PatientId { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? LeadId { get; set; }
        public string? Name { get; set; }
        public int? CurrentCounselorId { get; set; }   
        public string? CurrentCounselorName { get; set; } 
        public List<MessageResponseDto> Messages { get; set; } = new();
    }
}
