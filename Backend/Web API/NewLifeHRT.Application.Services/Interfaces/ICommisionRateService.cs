using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface ICommisionRateService
    {
        Task<List<CommisionRateGetAllResponseDto>> GetAllCommisionRatesAsync();
        Task<CommisionRateGetByIdResponseDto> GetCommisionRateByIdAsync(Guid id);
        Task<List<CommisionRateByProductIdResponseDto>> GetCommisionRateByProductIdAsync(Guid productId);
        Task<CommonOperationResponseDto<Guid>> CreateCommisionRateAsync(CommisionRateRequestDto dto, int userId);
        Task<CommonOperationResponseDto<Guid>> UpdateCommisionRateAsync(Guid id, CommisionRateRequestDto dto, int userId);
        Task DeleteCommisionRateAsync(List<Guid> ids, int userId);
        Task ActivateCommisionRateAsync(List<Guid> ids, int userId);
        Task DeactivateCommisionRateAsync(List<Guid> ids, int userId);
    }
}