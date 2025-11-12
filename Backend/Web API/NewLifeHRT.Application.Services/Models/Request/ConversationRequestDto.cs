using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class ConversationRequestDto
    {
        public Guid? PatientId { get; set; }
        public Guid? LeadId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public MessageRequestDto Message { get; set; }
        public MessageContentRequestDto MessageContent { get; set; }

    }
}
