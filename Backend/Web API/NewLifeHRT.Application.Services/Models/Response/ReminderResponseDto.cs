using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ReminderResponseDto
    {
        public Guid Id { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public string ReminderType { get; set; }
        public string? Description { get; set; }
    }
}
