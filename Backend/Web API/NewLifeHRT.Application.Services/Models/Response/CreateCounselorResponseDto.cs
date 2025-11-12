using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CreateCounselorResponseDto
    {
        public Guid? CounselorNoteId { get; set; }
        public string Message { get; set; }
    }
}
