using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class ReminderDashboardResponseDto
    {
        public Guid ReminderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ReminderDateTime { get; set; }
        public string ReminderTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceEndDate { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? LeadId {  get; set; }
    }
}
