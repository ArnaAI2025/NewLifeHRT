using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CreateResponseDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty; 
    }
}
