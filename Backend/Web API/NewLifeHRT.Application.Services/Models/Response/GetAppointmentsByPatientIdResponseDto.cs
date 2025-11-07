using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class GetAppointmentsByPatientIdResponseDto
    {
        public Guid AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string ServiceName { get; set; }
        public string CounselorName { get; set; }
        public string DoctorStartDateTime { get; set; }
        public string DoctorEndDateTime { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public DateTime UtcStartDateTime { get; set; }
        public DateTime UtcEndDateTime { get; set; }
        public string PatientName { get; set; }
    }
}
