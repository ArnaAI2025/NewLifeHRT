using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class MessageContentRequestDto
    {
        public Guid? MessageId { get; set; }
        public string? ContentType { get; set; }
        public string? Content { get; set; }
    }
}
