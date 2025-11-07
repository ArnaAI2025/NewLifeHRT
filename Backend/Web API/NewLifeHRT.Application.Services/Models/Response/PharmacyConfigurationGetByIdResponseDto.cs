using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmacyConfigurationGetByIdResponseDto
    {
        public Guid PharmacyId { get; set; }
        public int TypeId { get; set; }
        public string Status { get; set; }
        public List<PharmacyConfigurationKeyValueDto> ConfigData { get; set; } = new();
    }
}
