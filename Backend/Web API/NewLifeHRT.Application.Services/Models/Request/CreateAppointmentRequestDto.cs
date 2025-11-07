using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Request
{
    public class CreateAppointmentRequestDto
    {
        public Guid SlotId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public Guid PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ModeId { get; set; }
        public string? Description { get; set; }
    }
}
