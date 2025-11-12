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
    public interface IBatchMessageRecipientService
    {
        Task<BulkOperationResponseDto> CreateAsync(Guid batchMessageId, IList<BatchMessageRecipientRequestDto> request, int userId);
        Task<BulkOperationResponseDto> UpdateAsync(Guid batchMessageId, int status, int userId);
    }
}
