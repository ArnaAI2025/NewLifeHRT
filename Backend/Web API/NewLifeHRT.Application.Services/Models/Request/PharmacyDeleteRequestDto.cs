using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class PharmacyDeleteRequestDto
    {
        public List<Guid> PharmacyIds { get; set; }
    }
}
