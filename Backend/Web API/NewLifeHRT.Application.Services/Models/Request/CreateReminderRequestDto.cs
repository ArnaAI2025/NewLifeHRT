using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreateReminderRequestDto
    {
        public DateTime ReminderDateTime { get; set; }
        public int ReminderTypeId { get; set; }
        public string? Description { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurrenceRuleId { get; set; }
        public DateTime? RecurrenceEndDateTime { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? PatientId { get; set; }
    }
}
