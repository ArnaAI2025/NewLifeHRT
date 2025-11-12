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
    public interface IBatchMessageService
    {
        Task<BatchMessageResponseDto?> GetByIdAsync(Guid id);
        Task<List<BatchMessageResponseDto>?> GetAllAsync();
        Task<CommonOperationResponseDto<Guid>> CreateAsync(BatchMessageRequestDto request, int userId);
        Task<CommonOperationResponseDto<Guid>> UpdateAsync(Guid id, BatchMessageRequestDto request, int status, int userId);
        Task<BulkOperationResponseDto> BulkDeleteAsync(IList<Guid> batchMessages);
    }
}
