using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreatePatientSelfReminderRequestDto
    {
        public DateTime ReminderDateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
