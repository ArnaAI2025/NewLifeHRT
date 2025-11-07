using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface ICommissionsPayableService
    {
        Task<List<CommissionsPayableResponse>> GetCommissionByPoolDetailIdAsync(Guid poolDetailId);
        Task<CommissionsPayableDetailResponse?> GetCommissionByIdAsync(Guid commissionsPayableId);
        Task<CommonOperationResponseDto<Guid>?> InsertAsync(CommissionsPayable commissionsPayable);
        Task<CommonOperationResponseDto<Guid>> UpdateStatusCommissionPaybale(Guid orderId, int userId);
        Task<bool> HasCommissionEntryAsync(Guid orderId, CommissionEntryTypeEnum entryType);
    }
}
