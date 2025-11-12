using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class UpdateScheduleTimeRequestDto
    {
        public string Time { get; set; } = string.Empty;
        public List<Guid> ScheduleIds { get; set; } = new List<Guid>();
    }
}
