using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmacyConfigurationResponseDto
    {
        public Guid PharmacyConfigurationId { get; set; }
        public List<Guid> PharmacyConfigurationDataIds { get; set; } = new();
        public string Message { get; set; }
    }
}
