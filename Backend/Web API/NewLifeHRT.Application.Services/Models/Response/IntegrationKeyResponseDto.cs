using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class IntegrationKeyResponseDto
    {
        public int Id { get; set; }
        public int IntegrationTypeId { get; set; }
        public string KeyName { get; set; }
        public string Label { get; set; }
    }
}
