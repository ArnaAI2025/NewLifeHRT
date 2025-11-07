using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class CounselorNoteResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public bool IsAdminMailSent { get; set; }
        public bool IsDoctorMailSent { get; set; }
        public bool IsActive { get; set; }
    }
}
