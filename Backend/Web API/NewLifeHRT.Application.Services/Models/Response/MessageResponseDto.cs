using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class MessageResponseDto
    {
        public Guid MessageId { get; set; }
        public Guid? Id { get; set; }
        public int? CounselorId { get; set; }
        public string? CounselorName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TwilioId { get; set; }
        public string Direction { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsPatient { get; set; }
        public string? Name { get; set; }
        //public string? PatientName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool? IsSent { get; set; }
        public List<MessageContentResponseDto> MessageContents { get; set; } = new();
    }
}
