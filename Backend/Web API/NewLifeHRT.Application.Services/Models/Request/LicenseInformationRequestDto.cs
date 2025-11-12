using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class LicenseInformationRequestDto
    {
        public int StateId { get; set; }
        public int? Id { get; set; }
        public string Number { get; set; }
    }
}
