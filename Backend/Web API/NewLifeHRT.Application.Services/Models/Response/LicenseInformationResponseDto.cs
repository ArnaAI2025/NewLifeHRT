using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class LicenseInformationResponseDto
    {
        public Guid Id { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string? Number { get; set; }
    }
}
