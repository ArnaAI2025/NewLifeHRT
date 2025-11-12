using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Models.Response
{
    public class MedicalRecommendationResponseDto
    {

        public DateTime ConsultationDate { get; set; }
        public Guid PatientId { get; set; }
        public Guid Id { get; set; }
        public int DoctorId { get; set; }
        public int MedicationTypeId { get; set; }
        public string? OtherMedicationType { get; set; }

        public string? MedicationTypeName { get; set; }
        public int? FollowUpLabTestId { get; set; }
        public string? Title { get; set; }
        public string? PMHx { get; set; }
        public string? PSHx { get; set; }
        public string? FHx { get; set; }
        public string? Suppliments { get; set; }
        public string? Medication { get; set; }
        public string? SocialHistory { get; set; }
        public string? Allergies { get; set; }
        public string? HRT { get; set; }
        public string? Subjective { get; set; }
        public string? Objective { get; set; }
        public string? Assessment { get; set; }
        public string? Plan { get; set; }
        public string? SocialPoint { get; set; }
        public string? Notes { get; set; }
    }
}
