using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPatientService
    {
        Task<List<PatientResponseDto>> GetAllAsync();
        Task<List<CommonDropDownResponseDto<Guid>>> GetAllActiveAsync();
        Task<PatientResponseDto?> GetPatientByIdAsync(Guid id);
        Task<CommonOperationResponseDto<Guid?>> CreateAsync(CreatePatientRequestDto request, int? userId);
        Task<CommonOperationResponseDto<Guid>> ToggleActiveStatusAsync(Guid id, int userId, bool action);
        Task<CommonOperationResponseDto<Guid?>> UpdateAsync(string patientId, CreatePatientRequestDto request, int? userId);
        Task<BulkOperationResponseDto> BulkTogglePatientStatusAsync(List<string> patientIds, int userId, bool isActivating);
        Task<CommonOperationResponseDto<List<Guid>>> CreateMultipleAsync(IEnumerable<CreatePatientRequestDto> requests, int? userId);
        Task<CommonOperationResponseDto<int>> BulkAssignPatientsAsync(IEnumerable<Guid> patientIds, int assigneeId, int userId);
        Task BulkDeletePatientsAsync(List<Guid> patientIds, int userId);
        Task<Patient?> GetPatientByMobileNumber(string mobileNumber);
        Task<List<PatientLeadCommunicationDropdownDto>> GetAllOnCounselorIdAsync(int counselorId);
        Task<List<PatientCounselorInfoDto>> GetAllPatientsCounselorInfo();
        Task<DropDownIntResponseDto?> GetPhysicianNameByPatientIdAsync(Guid patientId);
        Task DeletePatientsAndLinkedUsersAsync(List<Guid> ids, int userId);
        Task<CommonOperationResponseDto<Guid?>> UpdatePatientAndUserAsync(string patientId, CreatePatientRequestDto request, int updaterUserId);
        Task<CommonOperationResponseDto<Guid?>> CreatePatientAndUserAsync(CreatePatientRequestDto request, int creatorUserId);
        Task<string> GenerateUniquePatientNumberAsync();
    }
}
