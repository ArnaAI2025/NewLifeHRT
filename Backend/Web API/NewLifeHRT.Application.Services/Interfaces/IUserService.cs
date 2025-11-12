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
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync(int? roleId);
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<CommonOperationResponseDto<int>> CreateAsync(CreateUserRequestDto createUserRequestDto, int userId);
        Task<CommonOperationResponseDto<int>> UpdateAsync(int  id, UpdateUserRequestDto updateUserRequestDto, int userId);
        Task<CommonOperationResponseDto<int>> PermanentDeleteAsync(int id, int userId);
        Task<List<DropDownIntResponseDto>> GetAllActiveUsersAsync(int roleId);
        Task<BulkOperationResponseDto> BulkToggleUserStatusAsync(List<int> userIds, int userId, bool isActivating);
        Task<List<DropDownIntResponseDto>> GetActiveUsersDropDownAsync(int roleId, string searchTerm = "");

        Task<List<DropDownIntResponseDto>> GetUsersOnVacationAsync();
        Task DeleteUsersAsync(List<int> userIds, int userId);
        Task<List<int>> GetUserIdsByPatientIdsAsync(List<Guid> patientIds);
    }
}
