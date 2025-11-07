using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class OtpVerificationRequestDto
    {
        public Guid OtpId { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
