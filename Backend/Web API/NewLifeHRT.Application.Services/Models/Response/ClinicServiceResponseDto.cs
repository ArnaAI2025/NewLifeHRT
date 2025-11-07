using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ClinicServiceResponseDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
    }
}
