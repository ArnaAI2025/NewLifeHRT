using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreateHolidayRequestDto
    {
        public int UserId { get; set; }
        public LeaveTypeEnum LeaveType { get; set; }
        public string? Description { get; set; }
        public List<HolidayDateRequestDto>? HolidayDates { get; set; }
        public HolidayRecurrenceRequestDto? Recurrence { get; set; }
    }
}
