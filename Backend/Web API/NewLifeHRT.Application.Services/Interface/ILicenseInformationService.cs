using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface ILicenseInformationService
    {
        Task<CommonOperationResponseDto<int>> CreateLicenseInformationAsync(LicenseInformationRequestDto[] request, int applicationUserId, int userId);
        Task<CommonOperationResponseDto<int>> UpdateLicenseInformationAsync(LicenseInformationRequestDto[] request, int applicationUserId, int userId);
    }
}
