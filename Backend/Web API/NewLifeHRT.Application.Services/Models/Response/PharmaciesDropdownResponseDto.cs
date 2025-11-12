using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class PharmaciesDropdownResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsLab { get; set; }

    }
}
