using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class MessageRequestDto
    {
        public Guid? ConversationId { get; set; }
        public int? UserId { get; set; }
        public string? TwilioId { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Direction { get; set; }
        public bool? IsSent { get; set; }
    }
}
