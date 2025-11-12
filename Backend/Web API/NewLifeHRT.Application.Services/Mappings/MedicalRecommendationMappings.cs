using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Mappings
{
    public static class MedicalRecommendationMappings
    {
        public static MedicalRecommendationResponseDto ToMedicalRecommendationResponseDto(this MedicalRecommendation medicalRecommendation)
        {
            

            return new MedicalRecommendationResponseDto
            {
                Id = medicalRecommendation.Id,
                ConsultationDate = medicalRecommendation.ConsultationDate,
                PatientId = medicalRecommendation.PatientId,
                DoctorId = medicalRecommendation.DoctorId,
                MedicationTypeId = medicalRecommendation.MedicationTypeId,
                OtherMedicationType = medicalRecommendation.OtherMedicationType,
                FollowUpLabTestId = medicalRecommendation.FollowUpLabTestId,

                Title = medicalRecommendation.Title,
                PMHx = medicalRecommendation.PMHx,
                PSHx = medicalRecommendation.PSHx,
                FHx = medicalRecommendation.FHx,
                Suppliments = medicalRecommendation.Suppliments,
                Medication = medicalRecommendation.Medication,
                SocialHistory = medicalRecommendation.SocialHistory,
                Allergies = medicalRecommendation.Allergies,
                HRT = medicalRecommendation.HRT,
                MedicationTypeName = medicalRecommendation?.MedicationType?.MedicationTypeName,
                Subjective = medicalRecommendation.Subjective,
                Objective = medicalRecommendation.Objective,
                Assessment = medicalRecommendation.Assessment,
                Plan = medicalRecommendation.Plan,
                SocialPoint = medicalRecommendation.SocialPoint,
                Notes = medicalRecommendation.Notes,
                
            };
        }
    }

}
