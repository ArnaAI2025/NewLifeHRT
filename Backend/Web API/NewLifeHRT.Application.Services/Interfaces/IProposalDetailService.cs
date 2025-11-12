using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface IProposalDetailService
    {
        Task<BulkOperationResponseDto> CreateProposalDetailAsync(List<ProposalDetailRequestDto> dto, Guid proposalId, int userId);
        Task<BulkOperationResponseDto> UpdateProposalDetailsAsync(List<ProposalDetailRequestDto> dtoList, Guid proposalId, int userId);
        Task<BulkOperationResponseDto> BulkToggleActiveStatusAsync(IList<Guid> proposalIds, int userId, bool isActive);
    }
}
