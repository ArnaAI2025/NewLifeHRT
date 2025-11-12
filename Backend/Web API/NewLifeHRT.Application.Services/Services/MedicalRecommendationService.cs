using Microsoft.AspNetCore.Http.HttpResults;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Mappings;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Services
{
    public class MedicalRecommendationService : IMedicalRecommendationService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalRecommendationRepository _medicalRecommendationRepository;

        public MedicalRecommendationService(
            IPatientRepository patientRepository,
            IMedicalRecommendationRepository medicalRecommendationRepository)
        {
            _patientRepository = patientRepository;
            _medicalRecommendationRepository = medicalRecommendationRepository;
        }

        public async Task<CommonOperationResponseDto<Guid?>> CreateAsync(MedicalRecommendationRequestDto request, int userId)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            var medicalRecommendation = new MedicalRecommendation
            {
                ConsultationDate = request.ConsultationDate,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                MedicationTypeId = request.MedicationTypeId,
                OtherMedicationType = request.OtherMedicationType,
                FollowUpLabTestId = request.FollowUpLabTestId,
                Title = request.Title,
                PMHx = request.PMHx,
                PSHx = request.PSHx,
                FHx = request.FHx,
                Suppliments = request.Suppliments,
                Medication = request.Medication,
                SocialHistory = request.SocialHistory,
                Allergies = request.Allergies,
                HRT = request.HRT,
                Subjective = request.Subjective,
                Objective = request.Objective,
                Assessment = request.Assessment,
                Plan = request.Plan,
                SocialPoint = request.SocialPoint,
                Notes = request.Notes,
                CreatedBy = userId.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            
                await _medicalRecommendationRepository.AddAsync(medicalRecommendation);
                await _medicalRecommendationRepository.SaveChangesAsync();            

            return new CommonOperationResponseDto<Guid?>
            {
                Id = medicalRecommendation.Id,
                Message = "MedicalRecommendation created successfully."
            };
        }

        public async Task<CommonOperationResponseDto<Guid>> UpdateAsync(MedicalRecommendationRequestDto request, int userId)
        {
            var medicalRecommendation = await _medicalRecommendationRepository.GetByIdAsync(request.Id);
            if (medicalRecommendation == null)
                throw new ArgumentNullException(nameof(medicalRecommendation));

            medicalRecommendation.ConsultationDate = request.ConsultationDate;
            medicalRecommendation.PatientId = request.PatientId;
            medicalRecommendation.DoctorId = userId;
            medicalRecommendation.MedicationTypeId = request.MedicationTypeId;
            medicalRecommendation.OtherMedicationType = request.MedicationTypeId == 8 ? request.OtherMedicationType : null;
            medicalRecommendation.FollowUpLabTestId = request.FollowUpLabTestId;
            medicalRecommendation.Title = request.Title;
            medicalRecommendation.PMHx = request.PMHx;
            medicalRecommendation.PSHx = request.PSHx;
            medicalRecommendation.FHx = request.FHx;
            medicalRecommendation.Suppliments = request.Suppliments;
            medicalRecommendation.Medication = request.Medication;
            medicalRecommendation.SocialHistory = request.SocialHistory;
            medicalRecommendation.Allergies = request.Allergies;
            medicalRecommendation.HRT = request.HRT;
            medicalRecommendation.Subjective = request.Subjective;
            medicalRecommendation.Objective = request.Objective;
            medicalRecommendation.Assessment = request.Assessment;
            medicalRecommendation.Plan = request.Plan;
            medicalRecommendation.SocialPoint = request.SocialPoint;
            medicalRecommendation.Notes = request.Notes;
            medicalRecommendation.UpdatedAt = DateTime.UtcNow;
            medicalRecommendation.UpdatedBy = userId.ToString();
            
                await _medicalRecommendationRepository.UpdateAsync(medicalRecommendation);
                await _medicalRecommendationRepository.SaveChangesAsync();           

            return new CommonOperationResponseDto<Guid>
            {
                Id = medicalRecommendation.Id,
                Message = "MedicalRecommendation updated successfully."
            };
        }

        public async Task<MedicalRecommendationResponseDto> GetByIdAsync(Guid id, int userId)
        {
            var medicalRecommendation = await _medicalRecommendationRepository.GetByIdAsync(id);
            if (medicalRecommendation == null)
                throw new ArgumentNullException(nameof(medicalRecommendation));
            return MedicalRecommendationMappings.ToMedicalRecommendationResponseDto(medicalRecommendation);
        }

        public async Task<List<MedicalRecommendationResponseDto>> GetAllByPatientIdAsync(Guid patientId)
        {
            var includes = new[] { "MedicationType" }; 
            var predicates = new List<Expression<Func<MedicalRecommendation, bool>>>
                        {
                            a => a.IsActive,
                            a => a.PatientId == patientId
                        };

            var recommendations = await _medicalRecommendationRepository.FindWithIncludeAsync(predicates, includes, noTracking: true);

            if (recommendations == null || !recommendations.Any())
                return new List<MedicalRecommendationResponseDto>();

            return recommendations
                .Select(MedicalRecommendationMappings.ToMedicalRecommendationResponseDto)
                .ToList();
        }


        public async Task<CommonOperationResponseDto<Guid>> SoftDeleteAsync(Guid id)
        {
            var medicalRecommendation = await _medicalRecommendationRepository.GetByIdAsync(id);

            if (medicalRecommendation == null)
                throw new ArgumentNullException($"MedicalRecommendation with Id {id} not found.");

            medicalRecommendation.IsActive = false;

            await _medicalRecommendationRepository.UpdateAsync(medicalRecommendation);
            await _medicalRecommendationRepository.SaveChangesAsync();
            return new CommonOperationResponseDto<Guid> { Message = "success"};
        }

    }


}
