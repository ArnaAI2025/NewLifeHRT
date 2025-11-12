using NewLifeHRT.Api.Requests;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IProposalService
    {
        Task<List<ProposalBulkResponseDto>> GetAllAsync(List<int>? ids, Guid? patientId);
        Task<CommonOperationResponseDto<Guid>> CreateProposalFromDtoAsync(ProposalRequestDto dto, int userId);
        Task<BulkOperationResponseDto> UpdateProposalAsync(Guid id, ProposalRequestDto dto, int userId);
        Task<BulkOperationResponseDto> BulkDeleteProposalAsync(IList<Guid> proposalIds);
        Task<ProposalResponseDto?> GetProposalById(Guid proposalId);
        Task<CommonOperationResponseDto<int>> BulkAssignProposalAsync(IEnumerable<Guid> proposalIds, int assigneeId, int userId);
        Task<CommonOperationResponseDto<Guid>> UpdateProposalStatusAsync(Guid proposalId, int status, string? description, int userId);
        Task<CommonOperationResponseDto<Guid>> UpdateProposalStatusToRejectByPatientAsync(Guid proposalId, int status, string? description, int userId);
        Task<CommonOperationResponseDto<Guid>> CloneOrderToProposalAsync(Guid orderId, int userId);
    }
}
