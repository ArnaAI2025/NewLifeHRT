using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IMessageService
    {
        Task<CommonOperationResponseDto<Guid>> CreateMessageAsync(MessageRequestDto dto, int userId, string createdBy);
        Task<List<MessageResponseDto>> GetUnReadMessagesByCounselorIdAsync(int userId);
        Task<BulkOperationResponseDto?> UpdateIsReadAsync(BulkOperationRequestDto<Guid> request, int userId);
        Task MarkUnreadInboundMessagesAsReadAsync(Guid conversationId, DateTime messageTimestamp, string updatedBy);
    }
}
