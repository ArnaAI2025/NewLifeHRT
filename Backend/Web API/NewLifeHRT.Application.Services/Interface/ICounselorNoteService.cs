using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface ICounselorNoteService
    {
        Task<List<CounselorNoteResponseDto>> GetAllActiveNotesBasesOnIdAsync(Guid id);
        Task<CommonOperationResponseDto<Guid?>> CreateAsync(CreateCounselorRequestDto request, int userId);
        Task<CommonOperationResponseDto<Guid>> DeleteNoteAsync(Guid id, int userId);
    }
}
