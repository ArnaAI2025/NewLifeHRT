using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Application.Services.Interface
{
    public interface IPharmacyConfigurationService
    {
        Task<List<CommonDropDownResponseDto<int>>> GetActiveIntegrationTypesAsync();
        Task<List<IntegrationKeyResponseDto>> GetIntegrationKeysByTypeIdAsync(int integrationTypeId);
        Task<PharmacyConfigurationResponseDto> CreatePharmacyConfigurationAsync(PharmacyConfigurationRequestDto request, int userId);
        Task<PharmacyConfigurationResponseDto> UpdatePharmacyConfigurationAsync(Guid pharmacyConfigurationId, PharmacyConfigurationRequestDto request, int userId);
        Task<BulkOperationResponseDto> DeletePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds);
        Task<BulkOperationResponseDto> ActivatePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds, int userId);
        Task<BulkOperationResponseDto> DeactivatePharmacyConfigurationsAsync(IList<Guid> pharmacyConfigurationIds, int userId);
        Task<List<PharmacyConfigurationGetAllResponseDto>> GetAllConfigurationsAsync();
        Task<PharmacyConfigurationGetByIdResponseDto?> GetConfigurationByIdAsync(Guid pharmacyConfigurationId);
    }
}
