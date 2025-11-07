using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PharmacyConfigurationRequestDto
    {
        public Guid PharmacyId { get; set; }
        public int TypeId { get; set; }
        public List<PharmacyConfigurationKeyValueDto> ConfigData { get; set; } = new();
    }
}
