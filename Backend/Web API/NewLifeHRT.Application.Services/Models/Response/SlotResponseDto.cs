using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class SlotResponseDto
    {
        public Guid SlotId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsBooked { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsAlreadyPassed { get; set; }
    }
}
