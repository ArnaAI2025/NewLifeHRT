using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IMedicalRecommendationService
    {
        Task<List<MedicalRecommendationResponseDto>> GetAllByPatientIdAsync(Guid patientId);
        Task<MedicalRecommendationResponseDto> GetByIdAsync(Guid id, int userId);
        Task<CommonOperationResponseDto<Guid?>> CreateAsync(MedicalRecommendationRequestDto request, int userId);
        Task<CommonOperationResponseDto<Guid>> UpdateAsync(MedicalRecommendationRequestDto request, int userId);
        Task<CommonOperationResponseDto<Guid>> SoftDeleteAsync(Guid id);
    }
}
