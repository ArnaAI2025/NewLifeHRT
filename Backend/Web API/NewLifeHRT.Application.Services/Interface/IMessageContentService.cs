using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IMessageContentService
    {
        Task<CommonOperationResponseDto<Guid>> CreateMessageContentAsync(MessageContentRequestDto dto, string userId);
        Task<List<MessageContent>> GetNonTextMessageContentsByPatientIdAsync(Guid patientId);
    }
}
