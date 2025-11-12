using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class OrderBulkResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PharmacyName { get; set; } = string.Empty;
        public string CounselorName { get; set; } = string.Empty;
        public DateTime? TherapyExpiration { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Status { get; set; }
    }
}
