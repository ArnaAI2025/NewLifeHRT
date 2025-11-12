using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interfaces
{
    public interface ICommissionsPayableService
    {
        Task<List<CommissionsPayableResponseDto>> GetCommissionByPoolDetailIdAsync(Guid poolDetailId);
        Task<CommissionsPayableDetailResponseDto?> GetCommissionByIdAsync(Guid commissionsPayableId);
        Task<CommonOperationResponseDto<Guid>?> InsertAsync(CommissionsPayable commissionsPayable);
        Task<CommonOperationResponseDto<Guid>> UpdateStatusCommissionPaybale(Guid orderId, int userId);
        Task<bool> HasCommissionEntryAsync(Guid orderId, CommissionEntryTypeEnum entryType);
    }
}
