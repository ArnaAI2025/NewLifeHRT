using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class AppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }
        public Guid SlotId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        public int ModeId { get; set; }
        public string ModeName { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public string? Description { get; set; }
        public string ServiceName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string CounselorName { get; set; }
        public string Title { get; set; }
        public DateTime DoctorStartDateTime { get; set; }
        public DateTime DoctorEndDateTime { get; set; }
        public string? ColorCode { get; set; }
    }
}
