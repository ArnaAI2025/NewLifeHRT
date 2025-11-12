using NewLifeHRT.Application.DTOs.Leads;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface ILeadService
    {
        Task<CommonOperationResponseDto<Guid?>> CreateAsync(LeadRequestDto createLeadRequestDto, int userId);
        Task<IList<LeadResponseDto>> GetAllAsync(int? ownerId = null);
        Task<LeadResponseDto?> GetByIdAsync(Guid id);
        Task<CommonOperationResponseDto<Guid?>> UpdateAsync(Guid id, LeadRequestDto updateLeadRequestDto, int userId);
        Task<CommonOperationResponseDto<int>> BulkToggleActiveStatusAsync(IEnumerable<Guid> leadIds, int userId, bool isActive);
        Task<CommonOperationResponseDto<int>> BulkAssignLeadsAsync(IEnumerable<Guid> leadIds, int assigneeId, int userId);
        Task<CommonOperationResponseDto<int>> BulkToggleIsQualifiedAsync(IEnumerable<Guid> leadIds, bool isQualified, int userId);
        Task<List<CreatePatientRequestDto>> ConvertToPatientRequestAsync(IEnumerable<Guid> ids);
        Task BulkDeleteLeadsAsync(List<Guid> leadIds, int userId);
        Task<Lead?> GetLeadByMobileNumber(string mobileNumber);
        Task<List<PatientLeadCommunicationDropdownDto>> GetAllOnCounselorIdAsync(int counselorId);
    }
}
