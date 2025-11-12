using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class MessageContentResponseDto
    {
        public Guid ContentId { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}
