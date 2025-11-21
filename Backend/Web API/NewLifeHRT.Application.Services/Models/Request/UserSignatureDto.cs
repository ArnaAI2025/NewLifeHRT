using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UserSignatureDto
    {
        public int UserId { get; set; }
        public List<string>? SignaturePaths { get; set; }
    }
}
